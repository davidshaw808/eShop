using Common;
using Common.Interface;

namespace DataLayer.ClassHelpers.Extensions;

internal static class GenericServiceExtensions
{
    internal static void Generate<T>(this IElement<T> element)
    {
        element.Id = null;
        element.Key = Guid.NewGuid();
    }

    internal static void Generate<T>(this IEnumerable<IElement<T>> elements)
    {
        foreach (var element in elements)
        {
            element.Id = null;
            element.Key = Guid.NewGuid();
        }
    }
}
