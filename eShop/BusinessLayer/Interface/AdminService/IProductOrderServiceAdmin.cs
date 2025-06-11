using Common.Models.Immutable;

namespace BusinessLayer.Interface.Admin;

public interface IProductOrderServiceAdmin
{
    (decimal DebitAmount, decimal CreditAmount, string Message)? UpdateAfterSale(Order o);
}
