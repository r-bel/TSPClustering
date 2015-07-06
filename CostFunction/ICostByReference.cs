using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostFunction
{
    public interface ICostByReference<TObject, TCost> /*: ICostFunction<TObject, TCost>*/ where TObject : class
    {
        TCost Cost(TObject from, TObject to);
    }
}
