using System;
using System.Collections.Generic;
using System.Linq;

namespace CostFunction
{
    public class CostMatrixLowerDiagRow : ICostByIndex<double>
    {
        private double[][] decoratedMatrix;

        public CostMatrixLowerDiagRow(double[][] matrix)
        {
            this.decoratedMatrix = matrix;
        }

        public double Cost(int from, int to)
        {
            if (from == to)
                return 0.0;

            int volgnummer;

            if (from > to)
            {
                volgnummer = (from * (from - 1)) / 2 + to;
            }
            else
            {
                volgnummer = (to * (to - 1)) / 2 + from;
            }

            from = volgnummer / (decoratedMatrix.Length - 1);
            to = volgnummer % (decoratedMatrix.Length - 1);

            return decoratedMatrix[from][to];
        }

    }
}
