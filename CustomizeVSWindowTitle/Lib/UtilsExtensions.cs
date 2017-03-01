using System.Collections.Generic;
using System.Linq;

namespace ErwinMayerLabs.Lib {
    public static class UtilsExtensions {
        public static IEnumerable<T> GetRange<T>(this IList<T> array, int startIndex, int endIndex) {
            if (startIndex <= endIndex) {
                for (var j = startIndex; j <= endIndex; ++j) {
                    yield return array[j];
                }
            }
            else {
                for (var j = startIndex; j >= endIndex; --j) {
                    yield return array[j];
                }
            }
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] matches) {
            return matches.Any(source.Contains);
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> matches) {
            return matches.Any(source.Contains);
        }
    }
}