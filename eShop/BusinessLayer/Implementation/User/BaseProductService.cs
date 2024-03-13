using BusinessLayer.ClassHelpers;
using Common;

namespace BusinessLayer.Implementation.User
{
    public class BaseProductService
    {
        protected IEnumerable<SimpleProduct> GroupProducts(IEnumerable<Product> products)
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
                    Quantity = gp.Products.Count()
                })
                .ToArray();
        }
    }
}
