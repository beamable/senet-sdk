using System.Collections.Generic;
using System.Linq;

namespace Assets.Senet.Scripts.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));
    }
}
