using Common.Interface;

namespace DataLayer.Interface.General;

/// <summary>
/// interface for untrcaked atomic one-off data base interactions - changes are implemented immediately
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IUnitOfWorkCUD<T>
{
    /// <summary>
    /// generates a an assiciated record in the database, 
    /// </summary>
    /// <param name="t"></param>
    void GenerateElementInUoW(T t);

    /// <summary>
    /// pdate
    /// </summary>
    /// <param name="t"></param>
    void UpdateElementInUoW(T t);
    void LogicalDeleteElementInUow(T t);
}