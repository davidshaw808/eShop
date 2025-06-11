using Common.Base;
using Common.Interface;
using Common.Models.Immutable;
using Common.Models.Mutable;
using System.Net.Mail;

namespace BusinessLayer.Visitor;

public class EndpointValidation: IVisitorCaller<bool>
{
    /// <summary>
    /// validates that either a new address, no key - required fields, or existing address Key present
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public bool Visit(Address visitor) {
        var valid = visitor switch
        {
            { } when string.IsNullOrWhiteSpace(visitor.PostalCode) => false,
            { } when !visitor?.AddressLines?.Any() ?? true => false,
            { } when string.IsNullOrWhiteSpace(visitor.HouseNameNumber) => false,
            _ => true,
        };
        return visitor.Key == null && !valid;
    }

    /// <summary>
    /// Key is present, Order is system generated
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public bool Visit(Order visitor)
    {
        var valid = visitor switch 
        {
            { } when visitor.Products.Any(p => p.Key is null)
        }

    }

    /// <summary>
    /// Key is present, FinancialTransaction is system generated
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public bool Visit(FinancialTransaction visitor) => visitor.Key != null;

    public bool Visit(Product visitor)
    {
        var valid = visitor switch
        {
            { } when string.IsNullOrWhiteSpace(visitor.Name) => false,
            { } when string.IsNullOrWhiteSpace(visitor.Description) => false,
            { } when visitor.NumberInStock == null  => false,
            _ => true,
        };
        return valid;
    }

    public bool Visit(Customer visitor)
    {
        var valid = visitor switch {
            { } when string.IsNullOrWhiteSpace(visitor.Email) => false,
            { } when !visitor.Key.HasValue => false,
            { } when visitor.OrderHistory?.Any() ?? false => false,
            { } when !visitor.DateOfBirth.HasValue => false,
            _ => true,
        };

        if (!valid)
        {
            return false;
        }
        try
        {
            var mailAddress = new MailAddress(visitor.Email);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool Visit(AdminUser visitor)
    {
        throw new NotImplementedException();
    }

    public bool Visit(Category visitor)
    {
        throw new NotImplementedException();
    }

    public bool Visit(Customer visitor)
    {
        throw new NotImplementedException();
    }

    public bool Visit(HistoryLog visitor)
    {
        throw new NotImplementedException();
    }

    public bool Visit(Review visitor)
    {
        throw new NotImplementedException();
    }
}
