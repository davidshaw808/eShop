using Common.Interface;
using BusinessLayer.Interface.User;
using Common.Models.Mutable;

namespace BusinessLayer.Interface.Admin;

public interface IProductServiceAdmin : IGenerateUpdateDelete<Product>, IProductService
{
}
