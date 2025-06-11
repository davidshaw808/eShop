using Common.Models.Immutable;
using Common.Models.Mutable;
using DataLayer.Databases.Base.NonDomainEntites;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataLayer.Databases.Base;
/***********************************************************************/
/*TODO's
 * figure out the tables that require super access and plonk them
 under a different schema
*/
/***********************************************************************/

public class eShopBaseContext : DbContext
{
    internal static string ReadAndInsertOnlySchemaName => "ReadAndInsert";

    internal DbSet<Address> Addresses { get; set; }
    internal DbSet<Category> Categories { get; set; }
    internal DbSet<Customer> Customers { get; set; }
    internal DbSet<Order> Orders { get; set; }
    internal DbSet<Product> Products { get; set; }
    internal DbSet<FinancialTransaction> FinancialTransactions { get; set; }
    internal DbSet<HistoryLog> HistoryLogs { get; set; }
    internal DbSet<OrderUpdate> OrderUpdates { get; set; }
    internal DbSet<Review> Reviews { get; set; }
    internal DbSet<Shipping> Shipping { get; set; }
    internal DbSet<Vat> Vat { get; set; }

    //join table entities
    internal DbSet<OrderProduct> OrderProducts { get; set; }
    internal DbSet<CategoryProduct> CategoryProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);     
    }
}
