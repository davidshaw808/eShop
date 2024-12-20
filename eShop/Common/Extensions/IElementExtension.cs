using System.Runtime.CompilerServices;

namespace Common.Extensions
{
    public static class IElementExtension
    {
        public static T GetType<T>(this IElement<T> me)
    }
}
