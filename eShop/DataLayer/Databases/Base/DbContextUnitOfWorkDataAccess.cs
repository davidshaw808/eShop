using DataLayer.Interface;

namespace DataLayer.Databases.Base
{
    public class DbContextUnitOfWorkDataAccess: IUnitofWorkDataAccess
    {
        private readonly eShopBaseContext _db;
        private bool _disposed;

        public DbContextUnitOfWorkDataAccess(eShopBaseContext db)  
        { 
            _db = db;
            _disposed = false;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
