namespace TicketBookingProject.Server;

public enum PaymentMethod : byte
{
    CreditCard = 1,
    BankTransfer = 2,
    Momo = 3,
    ZaloPay = 4,
    VnPay = 5,
}

public enum PaymentStatus : byte
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Refunded = 3,
}

public enum RefundStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Completed = 3,
}
