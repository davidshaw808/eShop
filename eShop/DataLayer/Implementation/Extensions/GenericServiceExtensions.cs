using Common;

namespace DataLayer.Implementation.Extensions;

internal static class GenericServiceExtensions
{
    internal static void AssignParentToChildren(this Category t)
    {
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 15 };

        if (t.Children != null && t.Parent != null) 
        {
            Parallel.ForEach (source: t.Children, parallelOptions: parallelOptions, body: (child) => 
            {
                child.Parent = t.Parent;
                //t.Parent.Children ??= [];//should never happen as t should be a member of it's parents children
                t?.Parent?.Children?.Add(child);
            });
        }
    }
}
