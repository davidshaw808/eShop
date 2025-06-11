using DataLayer.Databases.Base;

namespace DataLayer.Interface.General;

public interface IDbContextUnitOfWorkDataAccess : IAsyncDisposable
{
    eShopBaseContext GetContext();
    Task BeginUnitOfWork();
    Task CancelUnitOfWork();
    Task CommitUnitOfWork();
}