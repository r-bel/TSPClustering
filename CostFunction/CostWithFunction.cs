using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostFunction
{
    public class CostWithFunction<T> : ICostByIndex<double>
    {
        private IList<T> items;
        private Func<T, T, double> function;

        public CostWithFunction(IList<T> items, Func<T,T,double> function)
        {
            this.items = items;
            this.function = function;
        }
        
        public double Cost(int from, int to)
        {
            if (from == to)
                return 0.0;

            return function(items[from], items[to]);
        }
    }
}
