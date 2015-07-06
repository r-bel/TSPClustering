using System;
using System.Collections.Generic;
using System.Linq;

namespace CostFunction
{
    public class CostAnalyzer<TCost> where TCost : IComparable<TCost>
    {        
        private IList<SortedList<int, TCost>> OrderedNeighboursFromTo { get; set; }
        private class Comparer : IComparer<int>
        {
            internal TCost[] items;

            //public Comparer(int dimension)
            //{
            //    items = new TCost[dimension];
            //}

            public Comparer(TCost[] items)
            {
                this.items = items;
            }

            public int Compare(int x, int y)
            {
                if (x == y)
                    return 0;

                var retval = items[x].CompareTo(items[y]);
                if (retval == 0)
                    return x.CompareTo(y);
                else
                    return retval;
            }
        }

        public ICostMatrix<TCost> CostMatrix { get; private set; }

        public CostAnalyzer(ICostMatrix<TCost> costMatrix)
        {
            CostMatrix = costMatrix;

            OrderedNeighboursFromTo = new SortedList<int, TCost>[costMatrix.Matrix.Length];

            for (int row = 0; row < costMatrix.Matrix.Length; row++)
            {
                OrderedNeighboursFromTo[row] = new SortedList<int, TCost>(costMatrix.Matrix.Length-1, new Comparer(costMatrix.Matrix[row]));

                for (int col = 0; col < costMatrix.Matrix.Length; col++)
                {
                    OrderedNeighboursFromTo[row].Add(col, costMatrix.Matrix[row][col]);
                }
            }
        }

        public int NeareastNeighborIndex(int from)
        {
            return OrderedNeighboursFromTo[from].First().Key;
        }
        
        public KeyValuePair<int, TCost> NeareastNeighbor(int from)
        {
            return OrderedNeighboursFromTo[from].First();
        }

        public SortedList<int, TCost> AllNeighborsOrdered(int from)
        {
            return OrderedNeighboursFromTo[from];
        }

        public IList<int> AllNeighborsIndexesFromNearToFar(int from)
        {
            return OrderedNeighboursFromTo[from].Keys;
        }
    }
}
