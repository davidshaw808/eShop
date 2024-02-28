using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Implementation
{
    public class ProductOrderService: IProductOrderServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess;

        public ProductOrderService(IProductDataAccess productDataAccess)
        {
            this._productDataAccess = productDataAccess;
        }

        public (decimal RefundAmount, string Message)? UpdateAfterSale(Order order)
        {
            var error = new StringBuilder();
            decimal refundAmmount = 0;
            if (order.Products == null)
            {
                return null;
            }
            var prodList = GetProducsFromOrder(order.Products);
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
                this._productDataAccess.UpdateAll(order.Products);
                return null;
            }
            return (refundAmmount, error.ToString());
        }

        protected static IEnumerable<SimpleProduct> GetProducsFromOrder(IEnumerable<Product> products)
        {
            var groupedProds = products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = p.ToList() });

            return groupedProds
                .Select(gp => new SimpleProduct()
                {
                    Id = gp.Id,
                    Name = products.FirstOrDefault(p => p.Id == gp.Id)?.Name ?? "",
                    Price = gp.Products.Average(p => p.Price),
                    Quantity = gp.Products.Sum(p => p.NumberInStock)
                })
                .ToArray();
        }
    }
}
