using Common;

namespace DataLayer.Databases.Base.NonDomainEntites
{
    internal class OrderProduct
    {
        internal int OrderId { get; set; }
        internal int ProductId { get; set; }
        internal Order Order { get; set; }
        internal Product product { get; set; }
    }
}
