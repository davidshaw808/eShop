using Common.Models.Mutable;

namespace BusinessLayer.Interface.Admin;

public interface IProductOrderServiceAdmin
{
    (decimal DebitAmount, decimal CreditAmount, string Message)? UpdateAfterSale(Order o);
}
