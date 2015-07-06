using Clustering;
using CostFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    public class ClusteredGraphToCostMatrix<TNode> where TNode : class
    {
        public CostMatrix<double> CostMatrix { get; private set; }

        public ClusteredGraphToCostMatrix(ICostByReference<TNode, double> costsUnclustered, IList<Cluster<TNode, double>> clusters)
        {
            var costsArrayClusters = new List<double[]>();

            foreach (var firstCluster in clusters)
            {
                var nodesFirstCluster = firstCluster.Nodes.Count() == 1 ? new[] { firstCluster.Nodes.First() } : new[] { firstCluster.Nodes.First(), firstCluster.Nodes.Last() };
                foreach (var firstnode in nodesFirstCluster)
                {
                    var costsForFirstNode = new List<double>();
                    
                    foreach (var secondCluster in clusters)
                    {
                        var nodesSecondCluster = secondCluster.Nodes.Count() == 1 ? new[] { secondCluster.Nodes.First() } : new[] { secondCluster.Nodes.First(), secondCluster.Nodes.Last() };
                        foreach (var secondnode in nodesSecondCluster)
                        {
                            if (!Object.ReferenceEquals(firstCluster, secondCluster))
                            {
                                var cost = costsUnclustered.Cost(firstnode, secondnode);
                                costsForFirstNode.Add(cost);
                            }
                            else
                            {
                                if (Object.ReferenceEquals(firstnode, secondnode))
                                {
                                    var cost = costsUnclustered.Cost(firstnode, secondnode);
                                    costsForFirstNode.Add(cost);
                                }
                                else
                                {
                                    if (Object.ReferenceEquals(firstnode, firstCluster.Nodes.First()))
                                    {
                                        costsForFirstNode.Add(firstCluster.TotalCost - firstCluster.TotalCostReverse < 0D ? 0D : firstCluster.TotalCost - firstCluster.TotalCostReverse);
                                    }
                                    else
                                    {
                                        costsForFirstNode.Add(firstCluster.TotalCostReverse - firstCluster.TotalCost < 0D ? 0D : firstCluster.TotalCostReverse - firstCluster.TotalCost);
                                    }
                                }
                            }
                        }                        
                    }
                    costsArrayClusters.Add(costsForFirstNode.ToArray());
                }
            }

            CostMatrix = new CostMatrix<double>(costsArrayClusters.ToArray());
        }
    }
}
