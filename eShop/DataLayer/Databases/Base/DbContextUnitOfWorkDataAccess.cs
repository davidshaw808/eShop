using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataLayer.Databases.Base
{
    public class DbContextUnitOfWorkDataAccess : IDbContextUnitOfWorkDataAccess
    {
        private static eShopBaseContext _db { 
            get
            {
                return _db;
            }
            set
            {
                _db ??= value;
            }
        }

        private static IDbContextTransaction? _transaction { get; set; }

        public DbContextUnitOfWorkDataAccess(eShopBaseContext db)
        {
            _db = db;
        }

        public virtual async Task BeginUnitOfWork()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public virtual async Task CancelUnitOfWork()
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync();
        }

        public virtual async Task CommitUnitOfWork()
        {
            await _db.SaveChangesAsync();
            if (_transaction is not null)
                await _transaction.CommitAsync();
        }

        public virtual eShopBaseContext GetContext() =>  _db;

        public async ValueTask DisposeAsync()
        {
            await CommitUnitOfWork(); //.ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}