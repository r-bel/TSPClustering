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

        private struct ClusterNode
        {
            public Cluster<TNode, double> cluster;
            public TNode node;
        }

        private IList<ClusterNode> ClusterEndPoints { get; set; }
        
        public ClusteredGraphToCostMatrix(ICostByReference<TNode, double> costsUnclustered, IList<Cluster<TNode, double>> clusters)
        {
            var costsArrayClusters = new List<double[]>();

            var relevantNodes = clusters.SelectMany(cl => cl.Nodes.Count() == 1 ?
                new[] { new ClusterNode { cluster = cl, node = cl.Nodes.First() } } :
                new[] { new ClusterNode { cluster = cl, node = cl.Nodes.First() }, new ClusterNode { cluster = cl, node = cl.Nodes.Last() } });

            ClusterEndPoints = relevantNodes.ToList();

            foreach (var first in ClusterEndPoints)
            {
                var costsForFirstNode = new List<double>();
                    
                foreach (var second in ClusterEndPoints)
                {
                    if (!Object.ReferenceEquals(first.cluster, second.cluster))
                    {
                        var cost = costsUnclustered.Cost(first.node, second.node)*100000;
                        costsForFirstNode.Add(cost);
                    }
                    else
                    {
                        if (Object.ReferenceEquals(first.node, second.node))
                        {
                            var cost = costsUnclustered.Cost(first.node, second.node);
                            costsForFirstNode.Add(cost);
                        }
                        else
                        {
                            double cost;
                            if (Object.ReferenceEquals(first, first.cluster.Nodes.First()))
                            {
                                cost = first.cluster.TotalCost - first.cluster.TotalCostReverse < 0D ? 0D : first.cluster.TotalCost - first.cluster.TotalCostReverse;
                            }
                            else
                            {
                                cost = first.cluster.TotalCostReverse - first.cluster.TotalCost < 0D ? 0D : first.cluster.TotalCostReverse - first.cluster.TotalCost;
                            }
                            costsForFirstNode.Add(cost);
                        }
                    }
                }                        
                
                costsArrayClusters.Add(costsForFirstNode.ToArray());
            }

            CostMatrix = new CostMatrix<double>(costsArrayClusters.ToArray());
        }

        public IList<TNode> ClusterPathToFullPath(IEnumerable<int> pathThroughClusters)
        {
            var routeReconstruction = new List<TNode>(); // Build the routing

            ClusterNode prevClusterNode = new ClusterNode { cluster = null, node = null };
            
            foreach (var idx in pathThroughClusters) // iterate over route among clusters
            {
                var clusternode = ClusterEndPoints[idx];

                if (prevClusterNode.cluster != null && object.ReferenceEquals(clusternode.cluster, prevClusterNode.cluster)) // Add in betweens
                {
                    var nodes = object.ReferenceEquals(clusternode.cluster.Nodes.Last(), clusternode.node) ? clusternode.cluster.Nodes : clusternode.cluster.Nodes.Reverse();

                    routeReconstruction.AddRange(nodes);
                }
                else if(clusternode.cluster.Nodes.Count() == 1)
                {
                    routeReconstruction.Add(ClusterEndPoints[idx].node);
                }

                prevClusterNode = clusternode;
            }

            return routeReconstruction;
        }
    }
}
