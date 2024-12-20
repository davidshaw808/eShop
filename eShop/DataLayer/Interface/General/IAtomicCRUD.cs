namespace DataLayer.Interface.General;

/// <summary>
/// interface for atomic data base interactions - no changes sholud persist after being called
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IAtomicCRUD<T>
{
    ValueTask GenerateAtomicAsync(T t);
    ValueTask<T?> ReadAtomicAsync(Guid key);
    ValueTask<int> UpdateAtomicAsync(T t);
    ValueTask<int> LogicalDeleteAtomicAsync(T t);
}
