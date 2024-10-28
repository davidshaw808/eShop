using DataLayer.Interface.General;

namespace DataLayer.Databases.Base
{
    public class DbContextUnitOfWorkDataAccess : IDbContextUnitOfWorkDataAccess
    {
        private static eShopBaseContext _db { get
            {
                return _db;
            }
            set
            {
                if(_db is null)
                    _db = value;
            }
        }

        public DbContextUnitOfWorkDataAccess(eShopBaseContext db) => _db = db;


        public virtual eShopBaseContext GetContext() =>  _db;

        public async ValueTask DisposeAsync()
        {
            await _db.SaveChangesAsync();//.ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}
