using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostFunction
{
    public class CostMatrixByRef<TKey, TCost> : ICostByReference<TKey, TCost> where TKey : class
    {
        private class ByRefEqualityComparer : IEqualityComparer<TKey>
        {
            public bool Equals(TKey x, TKey y)
            {
 	            return Object.ReferenceEquals(x, y);
            }

            public int GetHashCode(TKey obj)
            {
 	            return obj.GetHashCode();
            }
        }

        private ICostByIndex<TCost> weightmatrix;
        private IDictionary<TKey, int> mapper;

        public CostMatrixByRef(ICostByIndex<TCost> weightmatrix, IList<TKey> orderedKeyItems)
        {
            this.weightmatrix = weightmatrix;

            mapper = new Dictionary<TKey, int>(new ByRefEqualityComparer());

            for (int i = 0; i < orderedKeyItems.Count; i++)
                mapper.Add(orderedKeyItems[i], i);
        }

        public TCost Cost(TKey from, TKey to)
        {
            int row = mapper[from];
            int col = mapper[to];

            return weightmatrix.Cost(row, col);
        }
    }
}
