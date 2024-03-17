using DataLayer.Databases.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DataLayer.Databases
{
    public class InMemoryContext : eShopBaseContext
    {
        public string DbPath { get; }

        public InMemoryContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "eShop.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
            var ignoreWarnings = new EventId[] { RelationalEventId.AmbientTransactionWarning };//TransactionScope does not support a non-relational database
            options.ConfigureWarnings(warnConfig => warnConfig.Ignore(ignoreWarnings));
        }

    }
}
