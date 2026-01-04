namespace DataLayer.Interface.General;

public interface IAtomicImmutable<T>
{
    Task<T?> GetAtomicAsync(Guid id);
    Task<bool> GenerateAtomicAsync(T t);
}
