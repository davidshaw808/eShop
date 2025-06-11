using Common.Models.Mutable;

namespace BusinessLayer.Interface.User;

public interface IAddressOrderService
{
    bool Generate(Address t);
}
