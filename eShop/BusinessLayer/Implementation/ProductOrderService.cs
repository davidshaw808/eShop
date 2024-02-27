using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface;
using Common;
using DataLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementation
{
    public class ProductOrderService: IProductOrderService
    {
        readonly IProductDataAccess _productDataAccess;

        public ProductOrderService(IProductDataAccess productDataAccess)
        {
            this._productDataAccess = productDataAccess;
        }

        public (decimal RefundAmount, string Message)? UpdateAfterSale(Order o)
        {
            StringBuilder error = new StringBuilder();
            decimal refundAmmount = 0;
            if (o.Products == null)
            {
                return null;
            }
            var groupedProds = o.Products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = p.ToList() });

            var prodList = groupedProds
                .Select(gp => new SimpleProduct()
                {
                    Id = gp.Id,
                    Name = o.Products.FirstOrDefault(p => p.Id == gp.Id)?.Name ?? "",
                    Price = gp.Products.Average(p => p.Price),
                    Quantity = gp.Products.Sum(p => p.NumberInStock)
                })
                .ToArray();
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
                this._productDataAccess.UpdateAll(o.Products);
                return null;
            }
            return (refundAmmount, error.ToString());
        }
    }
}
