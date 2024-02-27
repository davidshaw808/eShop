using Common;
using DataLayer.Databases.Base;
using Microsoft.EntityFrameworkCore;

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
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
