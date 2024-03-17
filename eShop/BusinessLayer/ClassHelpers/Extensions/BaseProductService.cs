using BusinessLayer.ClassHelpers;
using Common;

namespace BusinessLayer.ClassHelpers.Extensions
{
    public static class BaseProductService
    {
        /// <summary>
        /// For an enumerable of products, where the same product may repeat, return an enumeration of unique
        /// products with a count of the number for each product
        /// </summary>
        /// <param name="products">the enumeration to 'flatten'</param>
        /// <returns>an enumeration of unique SimpleProduct with a count for each where a product repeats</returns>
        public static IEnumerable<SimpleProduct> GroupProducts(this IEnumerable<Product> products)
        {
            var groupedProds = products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = [.. p] });

            return groupedProds
                .Select(gp => new SimpleProduct()
                {
                    Id = gp.Id,
                    Name = gp.Products.FirstOrDefault().Name,
                    Price = gp.Products.FirstOrDefault().Price,
                    Quantity = gp.Products.Count()
                })
                .ToArray();
        }
    }
}
