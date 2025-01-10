using Common;

namespace DataLayer.Databases.Base.NonDomainEntites
{
    internal class CategoryProducts
    {
        internal int CategoryId { get; set; }
        internal int ProductId { get; set; }
        internal Category Category { get; set; }
        internal Product Product {  get; set; }
    }
}
