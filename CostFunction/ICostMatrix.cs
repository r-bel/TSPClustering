using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostFunction
{
    public interface ICostMatrix<TCost>
    {
        TCost[][] Matrix { get; }
    }
}
