using Common;
using Common.Base;
using Common.Interface;
using System.Net.Mail;

namespace BusinessLayer.CommonVisitor;

public class EndpointValidation: IVisitor<Address, bool>, IVisitor<Person, bool>, IVisitor<Order, bool>, IVisitor<FinancialTransaction, bool>
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
    /// validates that the key is present, Order is system generated
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public bool Visit(Order visitor) => visitor.Key != null;

    /// <summary>
    /// validates that the key is present, FinancialTransaction is system generated
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
        return visitor.Key == null && !valid;
    }

    public bool Visit(Person visitor)
    {
        var valid = visitor switch {
            { } when string.IsNullOrWhiteSpace(visitor.Email) => false,
            { } when !visitor.DateOfBirth.HasValue => false,
            { } when string.IsNullOrWhiteSpace(visitor.UserName) => false,
            _ => true,
        };

        if (visitor.Key == null && !valid)
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
}
