using Common.Interface;

namespace DataLayer.ClassHelpers.Extensions;

internal static class GenericServiceExtensions
{
    internal static void GenerateForDatabase<T>(this IElement<T> element)
    {
        element.Id = null;
        element.Key = Guid.NewGuid();
    }

    internal static void GenerateForDatabase<T>(this IEnumerable<IElement<T>> elements)
    {
        foreach (var element in elements)
        {
            element.Id = null;
            element.Key = Guid.NewGuid();
        }
    }

    internal static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> elements, IEnumerable<Func<T,bool>> conditions)
    {
        var numConditions = conditions.Count();
        if(!(elements.Any() && numConditions > 0))
        {
            return [[]];
        }
        var retValues = new List<T>[numConditions+1];

        foreach (var element in elements)
        {
            for(var i = numConditions-1; i >= 0; i--)
            {
                if(conditions.ElementAt(i)(element))
                {
                    retValues[i].Add(element);
                    break;
                }
                else if(i == 0)
                {
                    retValues[numConditions].Add(element);
                }
            }
        }

        return retValues;
    }
}
