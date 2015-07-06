using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CostFunction
{
    public class CostMatrix<TCost> : ICostByIndex<TCost>, ICostMatrix<TCost>
    {
        public TCost[][] Matrix { get; private set; }

        public CostMatrix(ICostByIndex<TCost> function, int dimension)
        {
            Matrix = new TCost[dimension][];

            for(int row =0; row < dimension;row++)
            {
                Matrix[row] = new TCost[dimension];
                for (int col = 0; col < dimension; col++)
                {
                    Matrix[row][col] = function.Cost(row, col);
                }
            }
        }

        public CostMatrix(TCost[,] costsAsNumbers)
        {   
            int dimension = costsAsNumbers.GetLength(0);

            Contract.Requires(dimension == costsAsNumbers.GetLength(1));

            Matrix = new TCost[dimension][];
            for (int row = 0; row < dimension; row++)
            {
                Matrix[row] = new TCost[dimension];
                for (int col = 0; col < dimension; col++)
                {
                    Matrix[row][col] = costsAsNumbers[row, col];
                }
            }
        }

        public CostMatrix(int dimension)
        {
            Matrix = new TCost[dimension][];
            for (int row = 0; row < dimension; row++)
            {
                Matrix[row] = new TCost[dimension];
            }
        }

        public CostMatrix(TCost[][] costsAsNumbers)
        {
            Matrix = costsAsNumbers;
        }

        public TCost Cost(int from, int to)
        {
            return Matrix[from][to];
        }

        public static CostMatrix<TCost> CreateTransposed(ICostMatrix<TCost> originalMatrix)
        {
            var dimension = originalMatrix.Matrix.Length;
            
            var transposed = new TCost[originalMatrix.Matrix.Length][];

            for (int col = 0; col < originalMatrix.Matrix.Length; col++)
            {
                transposed[col] = new TCost[dimension];
                for (int row = 0; row < originalMatrix.Matrix.Length; row++)
                    transposed[col][row] = originalMatrix.Matrix[row][col];
            }

            return new CostMatrix<TCost>(transposed);
        }
    }
}
