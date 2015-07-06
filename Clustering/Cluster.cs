using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iteration;
using CostFunction;

namespace Clustering
{
    public class Cluster<TNode>
    {
        public IEnumerable<TNode> Nodes { get; private set; }

        public double TotalCost { get; private set; }
        public double TotalCostReverse { get; private set; }

        public Cluster(IEnumerable<TNode> nodes, double totalCost, double totalCostReverse)
        {
            this.Nodes = nodes;
            this.TotalCost = totalCost;
            this.TotalCostReverse = totalCostReverse;
        }
    }
}
