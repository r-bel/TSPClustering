using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CostFunction;
using Iteration;
using Validation;

namespace Clustering.Algorithms
{
    public class LargeATSPPreClustering<TNode>
    {
        private class ClusterFactory
        {
            private IList<TNode> allNodes;
            private CostAnalyzer<double> costanalyzer;

            public IList<int> NodesAsIndexes { get; private set; }

            public List<Cluster<TNode>> ListOfClusters { get; private set; }
            
            public ClusterFactory(IList<TNode> allNodes, CostAnalyzer<double> costanalyzer)
            {
                this.allNodes = allNodes;
                this.costanalyzer = costanalyzer;
                // We work with the indexes. Nodes will be flagged as part of cluster by removing them from this list.
                NodesAsIndexes = Enumerable.Range(0, allNodes.Count).ToList();
                ListOfClusters = new List<Cluster<TNode>>();
            }

            public void CreateCluster(IList<int> clusterNodes)
            {
                if (clusterNodes.Count > 1) // don't add singleton clusters here
                {
                    double costOfCluster = clusterNodes.Tuples().Sum(t => costanalyzer.AllNeighborsOrdered(t.Item1)[t.Item2]);

                    double reverseCost = clusterNodes.Reverse().Tuples().Sum(n => costanalyzer.AllNeighborsOrdered(n.Item1)[n.Item2]);

                    foreach (var j in clusterNodes)
                        NodesAsIndexes.Remove(j); // accepted as final member of the cluster           

                    ListOfClusters.Add(new Cluster<TNode>(clusterNodes.Select(n => allNodes[n]), costOfCluster, reverseCost));
                }
            }

            public void CreateSingletonClusters()
            {
                foreach (var j in NodesAsIndexes)
                    ListOfClusters.Add(new Cluster<TNode>(new[] { allNodes[j] }, 0, 0));

                NodesAsIndexes.Clear();
            }
        }

        private const int maxFactorOfLoopCount = 4;
        private static Random randomizer = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        private static void ExpandClusterFromStartPoint(int startIndex, List<int> constructingCluster, bool atTail, IList<int> nodesAsIndexes, CostAnalyzer<double> costanalyzer, double maxCostOfNearestNeighbor, int maxNodesPerCluster)
        {
            int currentPointer = startIndex;

            bool foundNextNodeInChain;

            do
            {
                foundNextNodeInChain = false;

                var myNeighborsNotAlreadyInConstructingCluster =
                    costanalyzer.AllNeighborsOrdered(currentPointer).Where(neighbor => currentPointer != neighbor.Key && !constructingCluster.Contains(neighbor.Key));

                if (myNeighborsNotAlreadyInConstructingCluster.Any())
                {
                    var nearestNeighbor = myNeighborsNotAlreadyInConstructingCluster.FirstOrDefault(); // The closest neighbor

                    if (nearestNeighbor.Value <= maxCostOfNearestNeighbor
                        && nodesAsIndexes.Contains(nearestNeighbor.Key)) // No overlapping with other clusters. We stop if the next one is in another cluster.
                    {
                        if (constructingCluster.Count == 0)
                            constructingCluster.Add(currentPointer); // If we start construct a cluster don't forget to add the beginning node.

                        if (atTail)
                            constructingCluster.Add(nearestNeighbor.Key);
                        else
                            constructingCluster.Insert(0, nearestNeighbor.Key);

                        currentPointer = nearestNeighbor.Key; // Move the pointer to the neighbor

                        foundNextNodeInChain = true;
                    }
                }
            }
            while (foundNextNodeInChain && (maxNodesPerCluster < 0 || constructingCluster.Count < maxNodesPerCluster));
        }

        private static List<int> CalculateCluster(IList<int> nodesAsIndexes, CostAnalyzer<double> costanalyzer, CostAnalyzer<double> transposedCostanalyzer, double maxCostOfNearestNeighbor, int maxNodesPerCluster)
        {
            var constructingCluster = new List<int>();

            int startIndex = nodesAsIndexes[randomizer.Next(0, nodesAsIndexes.Count - 1)];
            
            ExpandClusterFromStartPoint(startIndex, constructingCluster, true, nodesAsIndexes, costanalyzer, maxCostOfNearestNeighbor, maxNodesPerCluster);

            ExpandClusterFromStartPoint(startIndex, constructingCluster, false, nodesAsIndexes, transposedCostanalyzer, maxCostOfNearestNeighbor, maxNodesPerCluster);            

            return constructingCluster;
        }

        public IList<Cluster<TNode>> Solve([NotNull] IList<TNode> Nodes, [NotNull] CostAnalyzer<double> costanalyzer, double maxCostOfNearestNeighbor, int maxNodesPerCluster = -1)
        {
            var clusterFactory = new ClusterFactory(Nodes, costanalyzer);

            var transposedCostMatrix = CostMatrix<double>.CreateTransposed(costanalyzer.CostMatrix);

            var transposedCostanalyzer = new CostAnalyzer<double>(transposedCostMatrix);
            

            // While unhandled indexes available and the loop runs for at most a certain amount of times, continue the algorithm
            for (var counter = 0; counter < Nodes.Count * maxFactorOfLoopCount && clusterFactory.NodesAsIndexes.Count > 0; counter++)
            {
                // set the current pointer to a random node index among the nodes not yet classified in a cluster
                int currentPointer = clusterFactory.NodesAsIndexes[randomizer.Next(0, clusterFactory.NodesAsIndexes.Count - 1)];

                var candidateCluster = CalculateCluster(clusterFactory.NodesAsIndexes, costanalyzer, transposedCostanalyzer, maxCostOfNearestNeighbor, maxNodesPerCluster);

                if (candidateCluster.Count > 1)
                {
                    // loop over the cluster again maybe to cut the cluster in pieces. Clusters with one item are thrown back in the pool
                    for (int indexInCluster = 0; indexInCluster < candidateCluster.Count - 1; indexInCluster++)
                    {
                        var maxCostInCluster = indexInCluster == 0 ? 
                            costanalyzer.AllNeighborsOrdered(candidateCluster[0])[candidateCluster[1]] 
                            : candidateCluster.Take(indexInCluster + 1).Tuples().Max(t => costanalyzer.AllNeighborsOrdered(t.Item1)[t.Item2]);

                        var nodesOutsideClusterQuiteCloseToCluster = costanalyzer.AllNeighborsOrdered(candidateCluster[indexInCluster])
                            .Where(neighbor => 
                                    !candidateCluster.Contains(neighbor.Key) && neighbor.Value <= maxCostInCluster);

                        if (nodesOutsideClusterQuiteCloseToCluster.Count() > 1) // Split the cluster
                        {
                            var firstPart = candidateCluster.GetRange(0, indexInCluster + 1);

                            clusterFactory.CreateCluster(firstPart);

                            candidateCluster.RemoveRange(0, indexInCluster + 1);
                            indexInCluster = -1;
                        }
                    }
                }

                clusterFactory.CreateCluster(candidateCluster);
            }

            // Here we add the singleton clusters from the remaining nodes
            clusterFactory.CreateSingletonClusters();

            return clusterFactory.ListOfClusters;
        }
    }
}
