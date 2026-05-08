using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;
using TicketBookingProject.Server.Models;
using static TicketBookingProject.Server.PaymentMappingProfile;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ITicketService _ticketService;
        private readonly IBookingService _bookingService;
        private readonly IEmailService _emailService;
        public PaymentController(IPaymentService paymentService, ITicketService ticektService, IBookingService bookingService, IEmailService emailService)
        {
            _paymentService = paymentService;
            _ticketService = ticektService;
            _bookingService = bookingService;
            _emailService = emailService;
        }

        [HttpPost("create-momo-payment")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] CreatePaymentRequest request)
        {
            var paymentUrl = await _paymentService.CreatePaymentIntentByMomo(request.Amount, request.BookingInfo, request.BookingId);
            if (paymentUrl == null)
            {
                return BadRequest("Failed to checkout by Momo!!!");
            }
            return Ok(ApiResponse<MomoResponse>.Ok(paymentUrl));
        }

        [HttpPost("create-vnpay-payment")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] CreatePaymentRequest request)
        {
            var paymentUrl = await _paymentService.CreatePaymentIntentByVnPay(request.Amount, request.BookingInfo, request.BookingId);
            if (paymentUrl == null)
            {
                return BadRequest("Failed to checkout by VnPay!!!");
            }
            return Ok(ApiResponse<VnPayResponse>.Ok(paymentUrl));
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayCallback()
        {
            var response = await _paymentService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode != "00")
            {
                var updateBookingStatus = await _bookingService.UpdateBookingStatus(int.Parse(response.OrderId), BookingStatus.Cancelled);
                await _bookingService.RegainQuantityTicketType(int.Parse(response.OrderId));
                return BadRequest(new { message = "Payment failed", data = response });
            }

            var payment = await _paymentService.CreatePaymentIntentByVnPayCallback(response);

            if (payment != null)
            {
                var bookingId = payment.BookingId;
                var updateBookingStatus = await _bookingService.UpdateBookingStatus(bookingId, BookingStatus.Confirmed);
                var booking = await _bookingService.GetBookingEmailResponseById(bookingId);
                var tickets = await _ticketService.CreateTickets(bookingId);
                if (tickets != null && tickets.Any())
                {
                    _ = Task.Run(async () => {
                        try
                        {
                            await _emailService.SendTicketEmailAsync(
                                booking != null ? booking.UserEmail : "tieukhuynhtu@gmail.com",
                                booking != null ? booking.CustomerName : "MaiHuy",
                                booking != null ? booking.Id.ToString() : "BK_056D",
                                tickets
                            );
                        }
                        catch (Exception ex)
                        {
                            //_logger.LogError($"Lỗi gửi mail: {ex.Message}");
                        }
                    });
                }
            }

            return Ok(new { message = "Retrived payment successful", data = response });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var vnpayData = HttpContext.Request.Query;
            var result = await _paymentService.PaymentExecute(vnpayData);
            var response = (new VnPayReturnResponseDto
            {
                Success = true,
                Message = "Retrived payment successful",
                OrderId = result.OrderId,
                Amount = result.TotalAmount,
                TransactionId = result.TransactionId
            });
            if (result.VnPayResponseCode == "00")
            {
                return Ok(ApiResponse<VnPayReturnResponseDto>.Ok(response));
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = "Payment failed or signmark does not valid",
                    data = result
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin,organizer")]
        [HasPermission("payment:manage")]
        public async Task<IActionResult> GetListPayment([FromQuery] AdminPaymentListRequest req)
        {
            var payments = await _paymentService.GetListPayment(req);

            return payments.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payments = await _paymentService.DeletePayment(id);

            return Ok(ApiResponse<bool>.Ok(payments, "Delete payment successfully!!!"));
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue([FromQuery] TotalVenueRequest req)
        {
            var totalRevenues = await _paymentService.GetListTotalRevenue(req);

            return Ok(ApiResponse<List<TotalVenueReponse>>.Ok(totalRevenues, "Load totalRevenues successfully!!!"));
        }
    }
}
