using Common.Models.Immutable;
using Common.Models.Mutable;

namespace BusinessLayer.ClassHelpers.Extensions;

internal static class GenericServiceExtension
{
    internal static void AssignToParent(this Category category)
    {
        if ((category.Children == null && category.Products == null) || category.Parent == null)
            return;
        //assign categories
        var categories = category.Parent.Children.Where(c => c.Key != category.Key).ToList();
        if (category.Children != null)
        {
            if (category.Children is List<Category> childList)
            {
                categories.AddRange(childList);
            }
            else
            {
                categories.AddRange(category.Children);
            }
        }
        if (category.Products == null)
            return;
        category.Parent.Children = categories;
        //assign products
        var products = category.Products.ToList();
        if (category.Parent.Products is List<Product> productList)
        {
            productList.AddRange(category.Products);
            return;
        }
        var pList = category.Parent.Products.ToList();
        pList.AddRange(category.Products);
        category.Parent.Products = pList;
    }
    /*
    var magicNumberOfChilderen = 15;
    var reassign = (Category parent, Category child) =>
    {
        child.Parent = category.Parent;
        //t.Parent.Children ??= [];//should never happen as t should be a member of it's parents children
        category?.Parent?.Children?.Add(child);
    };

    if(category.Children.Count < magicNumberOfChilderen)
    {
        foreach (var child in category.Children)
        {
            reassign (category, child);
        }
        return;
    }
    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = magicNumberOfChilderen };

    Parallel.ForEach (source: category.Children, parallelOptions: parallelOptions, body: (child) => 
    {
        reassign (category, child);
    });

}*/

}
