using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;
using System.Text;

namespace BusinessLayer.Implementation.Admin
{
    public class ProductOrderServiceAdmin(IProductDataAccess productDataAccess) : BaseProductService, IProductOrderServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess = productDataAccess;

        public (decimal RefundAmount, string Message)? UpdateAfterSale(Order order)
        {
            var error = new StringBuilder();
            decimal refundAmmount = 0;
            if (order.Products == null)
            {
                return null;
            }
            var prodList = GroupProducts(order.Products);
            foreach (var p in prodList)
            {
                if (p.Quantity > 0)
                {
                    p.Quantity--;
                }
                else
                {
                    error.AppendLine($"{p.Name} out of stock, order unfulfilled. Price {p.Price}");
                    refundAmmount += p.Price;
                }
            }
            if (error.Length == 0 && refundAmmount == 0)
            {
                _productDataAccess.UpdateAll(order.Products);
                return null;
            }
            return (refundAmmount, error.ToString());
        }
    }
}
