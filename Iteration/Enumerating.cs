using System;
using System.Collections.Generic;
using System.Linq;

namespace Iteration
{
    public static class Enumerating
    {
        public static IEnumerable<Tuple<T, T>> Tuples<T>(this IEnumerable<T> path)
        {
            var prev = path.FirstOrDefault();
            if (prev != null)
                foreach (var t in path.Skip(1))
                {
                    yield return new Tuple<T, T>(prev, t);
                    prev = t;
                }
        }
        
        public static IEnumerable<Tuple<T, T>> TourTuples<T>(this IEnumerable<T> path)
        {
            var prev = path.FirstOrDefault();
            if (prev != null)
                foreach (var t in path.Skip(1).Concat(new[] { path.First() }))
                {
                    yield return new Tuple<T, T>(prev, t);
                    prev = t;
                }
        }
    }
}
