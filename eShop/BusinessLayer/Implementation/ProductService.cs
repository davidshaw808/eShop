using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface;
using Common;
using DataLayer.Interface;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLayer.Implementation
{
    public class ProductService : IProductService
    {
        readonly IProductDataAccess _productDataAccess;

        public ProductService(IProductDataAccess productDataAccess)
        {
            this._productDataAccess = productDataAccess;
        }   

        public bool Delete(Product t)
        {
            if(t.Id == null)
            {
                return false;
            }
            this._productDataAccess.Delete(t);
            return true;
        }

        public bool Generate(Product t)
        {
            return this._productDataAccess.Generate(t);
        }

        public bool IsValidTransaction(IEnumerable<Product> products)
        {
            if(products.Any(p => p.Id == null || p.NumberInStock == null))
            {
                return false;
            }
            var groupedProds = products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = p.ToList() });

            var prodList = groupedProds
                .Select(gp => new SimpleProduct(){Id = gp.Id, Quantity = gp.Products.Sum(p => p.NumberInStock)})
                .ToArray();

            foreach (var p in prodList)
            {
                if (p.Quantity > 0)
                {
                    p.Quantity--;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool Update(Product t)
        {
            throw new NotImplementedException();
        }

        public (decimal RefundAmount, string Message)? UpdateAfterSale(Order o)
        {
            StringBuilder error = new StringBuilder();
            decimal refundAmmount = 0;
            if(o.Products == null)
            {
                return null;
            }
            var groupedProds = o.Products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = p.ToList() });

            var prodList = groupedProds
                .Select(gp => new SimpleProduct() {
                    Id = gp.Id,
                    Name = o.Products.FirstOrDefault(p => p.Id == gp.Id)?.Name ?? "",
                    Price = gp.Products.Average(p => p.Price),
                    Quantity = gp.Products.Sum(p => p.NumberInStock) })
                .ToArray();
            foreach (var p in prodList)
            {
                if(p.Quantity > 0)
                {
                    p.Quantity--;
                }
                else
                {
                    error.AppendLine($"{p.Name} out of stock, order unfulfilled. Price {p.Price}");
                    refundAmmount += p.Price;
                }
            }
            if(error.Length == 0 && refundAmmount == 0)
            {
                this._productDataAccess.UpdateAll(o.Products);
                return null;
            }
            return (refundAmmount, error.ToString());
        }
    }
}
