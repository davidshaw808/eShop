namespace Common.Interface;

public interface IGenerateUpdateDelete<T>
{
    Task<int> GenerateAsync(T t);
    Task<int> UpdateAsync(T t);
    Task<int> LogicalDeleteAsync(T t);
}