using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Azure.Core;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using TicketBookingProject.Server.Helpers;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _cfg;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IBookingService _bookingService;
    public readonly IMapper _mapper;
    public PaymentService(IConfiguration cfg, 
        IHttpContextAccessor httpContextAccessor, 
        IPaymentRepository paymentRepo, 
        ICurrentUserService currentUser, 
        IBookingService bookingService,
        IMapper mapper)
    {
        _cfg = cfg;
        _httpContextAccessor = httpContextAccessor;
        _paymentRepo = paymentRepo;
        _currentUser = currentUser;
        _bookingService = bookingService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AdminPaymentListItemResponse>> GetListPayment(AdminPaymentListRequest req)
    {
        var (payments, total) = await _paymentRepo.GetListPayment(req);

        var pagedPayments = await payments.ProjectTo<AdminPaymentListItemResponse>(_mapper.ConfigurationProvider).ToListAsync();

        return new PagedResponse<AdminPaymentListItemResponse>(
                pagedPayments,
                req.Page,
                req.PageSize,
                total
            );
    }

    //chưa kết nối được với momo business nên chưa test được
    public async Task<MomoResponse?> CreatePaymentIntentByMomo(long amount, string BookingInfo, string BookingId)
    {
        var partnerCode = _cfg["MomoConfig:PartnerCode"];
        var accessKey = _cfg["MomoConfig:AccessKey"];
        var secretKey = _cfg["MomoConfig:SecretKey"];
        var endpoint = _cfg["MomoConfig:PaymentUrl"];
        var redirectUrl = _cfg["MomoConfig:RedirectUrl"];
        var ipnUrl = _cfg["MomoConfig:IpnUrl"];
        var requestId = Guid.NewGuid().ToString();
        var requestType = "captureWallet";
        var extraData = "";
        BookingId = BookingId + "_" + DateTime.Now.Ticks;

        string rawHash = "accessKey=" + accessKey + "&amount=" + amount + "&extraData=" + extraData + "&ipnUrl=" + ipnUrl + "&orderId=" + BookingId + "&orderInfo=" + BookingInfo + "&partnerCode=" + partnerCode + "&redirectUrl=" + redirectUrl + "&requestId=" + requestId + "&requestType=" + requestType;

        QuickPayResquest request = new QuickPayResquest();
        request.orderInfo = BookingInfo;
        request.partnerCode = partnerCode;
        request.redirectUrl = redirectUrl;
        request.ipnUrl = ipnUrl;
        request.amount = amount;
        request.orderId = BookingId;
        request.requestId = requestId;
        request.requestType = requestType;
        request.extraData = extraData;
        request.partnerName = "MoMo Payment";
        request.storeId = "Test Store";
        request.orderGroupId = "";
        request.autoCapture = true;
        request.lang = "vi";
        request.signature = ComputeHmacSha256(rawHash, secretKey!);

        using var client = new HttpClient();
        StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var quickPayResponse = await client.PostAsync(endpoint, httpContent);
        //var contents = quickPayResponse.Content.ReadAsStringAsync().Result;

        var contents = await quickPayResponse.Content.ReadAsStringAsync();
        Console.WriteLine("MoMo response: " + contents);

        return await quickPayResponse.Content.ReadFromJsonAsync<MomoResponse>();
    }

    private string ComputeHmacSha256(string message, string secretKey)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
        return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
    }

    public async Task<VnPayResponse?> CreatePaymentIntentByVnPay(long amount, string BookingInfo, string BookingId)
    {
        string vnp_Returnurl = _cfg["VnPay:ReturnUrl"]!;
        string vnp_Url = _cfg["VnPay:BaseUrl"]!;
        string vnp_TmnCode = _cfg["VnPay:TmnCode"]!;
        string vnp_HashSecret = _cfg["VnPay:HashSecret"]!;

        var vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString()); // VNPay nhân 100 số tiền
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + BookingId);
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", BookingId);

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

        return new VnPayResponse
        {
            payUrl = paymentUrl
        };
    }

    public async Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections)
    {
        var vnpay = new VnPayLibrary();

        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnpay.AddResponseData(key, value.ToString());
            }
        }

        var vnp_SecureHash = collections["vnp_SecureHash"].ToString();
        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash!, _cfg["Vnpay:HashSecret"]!);

        if (!checkSignature) return new VnPaymentResponseModel { Success = false };

        var responseCode = collections["vnp_ResponseCode"].ToString();

        return new VnPaymentResponseModel
        {
            Success = responseCode == "00",
            PaymentMethod = "VnPay",
            TotalAmount = long.Parse(collections["vnp_Amount"]!) / 100,
            OrderDescription = collections["vnp_OrderInfo"].ToString(),
            OrderId = collections["vnp_TxnRef"].ToString(),
            TransactionId = collections["vnp_TransactionNo"].ToString(),
            Token = vnp_SecureHash,
            VnPayResponseCode = responseCode,
            MetaData = collections,
            IdempotencyKey = collections["vnp_TransactionNo"]
        };
    }

    public async Task<Payment?> CreatePaymentIntentByVnPayCallback(VnPaymentResponseModel response)
    {
        if (!response.Success) return null;
        var bookingId = int.Parse(response.OrderId.Split('_')[0]);
        var booking = await _bookingService.GetBookingByIdAsync(bookingId);
        if (booking == null)
            throw new Exception($"Booking {bookingId} không tồn tại");

        var payment = new Payment
        {
            UserId = booking.UserId,
            BookingId = int.Parse(response.OrderId.Split('_')[0]),
            TotalAmount = response.TotalAmount,
            PaymentMethod = PaymentMethod.VnPay,
            Status = response.VnPayResponseCode == "00" ? PaymentStatus.Success : PaymentStatus.Failed,
            PaymentTransaction = response.TransactionId,
            IdempotencyKey = response.IdempotencyKey,
            MetaData = JsonSerializer.Serialize(response.MetaData),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        await _paymentRepo.CreatePaymentAsync(payment);

        return payment;
    }

    public async Task<bool> DeletePayment(int id)
    {
        var payment = await _paymentRepo.GetPaymentById(id);

        if (payment == null) return false;
        var deleted = await _paymentRepo.DeletePayment(payment);

        return deleted;
    }
}
