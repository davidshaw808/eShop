namespace DataLayer.Interface.General;

public interface IAtomicCRUD<T>
{
    Task GenerateAtomicAsync(T t);
    Task<T?> ReadAtomicAsync(Guid key);
    Task<int> UpdateAtomicAsync(T t);
    Task<int> LogicalDeleteAtomicAsync(T t);
}
