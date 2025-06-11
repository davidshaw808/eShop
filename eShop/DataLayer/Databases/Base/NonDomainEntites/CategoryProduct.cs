using Common;

namespace DataLayer.Databases.Base.NonDomainEntites
{
    internal class CategoryProduct
    {
        internal int CategoryId { get; set; }
        internal int ProductId { get; set; }
        internal Category Category { get; set; }
        internal Product Product {  get; set; }
    }
}
