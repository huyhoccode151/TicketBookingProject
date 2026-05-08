using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IPaymentService
{
    Task<MomoResponse?> CreatePaymentIntentByMomo(long amount, string BookingInfo, string BookingId);
    Task<VnPayResponse?> CreatePaymentIntentByVnPay(long amount, string BookingInfo, string BookingId);
    Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections);
    Task<VnPayRefundResponse?> ExecuteRefundAsync(Payment payment);
    Task<Payment?> CreatePaymentIntentByVnPayCallback(VnPaymentResponseModel response);
    Task<Result<PagedResponse<AdminPaymentListItemResponse>>> GetListPayment(AdminPaymentListRequest req);
    Task<bool> DeletePayment(int id);
    Task<List<TotalVenueReponse>> GetListTotalRevenue(TotalVenueRequest req);
}
