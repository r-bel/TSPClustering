using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iteration;
using CostFunction;

namespace Clustering
{
    public class Cluster<TNode, TCost>
    {
        public IEnumerable<TNode> Nodes { get; private set; }

        public TCost TotalCost { get; private set; }
        public TCost TotalCostReverse { get; private set; }

        public Cluster(IEnumerable<TNode> nodes, TCost totalCost, TCost totalCostReverse)
        {
            this.Nodes = nodes;
            this.TotalCost = totalCost;
            this.TotalCostReverse = totalCostReverse;
        }
    }
}
