using Common;
using DataLayer.Databases.Base;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Databases
{
    public class InMemoryContext : CrispHabitatBaseContext
    {
        public string DbPath { get; }

        public InMemoryContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "CrispHabitat.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
