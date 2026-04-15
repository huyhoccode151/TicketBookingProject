using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
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
        public PaymentController(IPaymentService paymentService, ITicketService ticektService)
        {
            _paymentService = paymentService;
            _ticketService = ticektService;
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
                return BadRequest(new { message = "Thanh toán thất bại", data = response });
            }

            var payment = await _paymentService.CreatePaymentIntentByVnPayCallback(response);

            if (payment != null)
            {
                var bookingId = payment.BookingId;
                var tickets = await _ticketService.CreateTickets(bookingId);
            }

            return Ok(new { message = "Thanh toán thành công", data = response });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var vnpayData = HttpContext.Request.Query;
            var result = await _paymentService.PaymentExecute(vnpayData);
            var response = (new VnPayReturnResponseDto
            {
                Success = true,
                Message = "Thanh toán thành công",
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
                    message = "Thanh toán thất bại hoặc chữ ký không hợp lệ",
                    data = result
                });
            }
        }

        //admin
        [HttpGet]
        public async Task<IActionResult> GetListPayment([FromQuery] AdminPaymentListRequest req)
        {
            var payments = await _paymentService.GetListPayment(req);

            return Ok(ApiResponse<PagedResponse<AdminPaymentListItemResponse>>.Ok(payments, "Load tickets successfully!!!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payments = await _paymentService.DeletePayment(id);

            return Ok(ApiResponse<bool>.Ok(payments, "Delete payment successfully!!!"));
        }
    }
}
