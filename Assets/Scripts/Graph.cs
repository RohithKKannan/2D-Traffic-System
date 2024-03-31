using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;

namespace TS
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Node nodePrefab;

        private List<Node> nodes = new();
        private int nodeCount;

        public UnityAction<int> OnNodeSelect;
        public UnityAction<Node, Node> OnNodeClickComplete;

        private Node selectedNode1;
        private Node selectedNode2;

        private bool isClicking;

        private void Awake()
        {
            gameManager.UIManager.AddNodeButton.onClick.AddListener(AddNewNode);
            gameManager.UIManager.RemoveNodeButton.onClick.AddListener(RemoveNode);
            gameManager.UIManager.ClearNodesButton.onClick.AddListener(ClearNodes);
            gameManager.UIManager.DrawModeButton.onClick.AddListener(EnterDrawMode);
            gameManager.UIManager.RemoveModeButton.onClick.AddListener(EnterRemoveMode);
            gameManager.UIManager.GetWeightButton.onClick.AddListener(GetWeightMode);
            gameManager.UIManager.ShortestPathButton.onClick.AddListener(FindShortestPath);
        }

        private void OnDestroy()
        {
            gameManager.UIManager.AddNodeButton.onClick.RemoveListener(AddNewNode);
            gameManager.UIManager.RemoveNodeButton.onClick.RemoveListener(RemoveNode);
            gameManager.UIManager.ClearNodesButton.onClick.RemoveListener(ClearNodes);
            gameManager.UIManager.DrawModeButton.onClick.RemoveListener(EnterDrawMode);
            gameManager.UIManager.RemoveModeButton.onClick.RemoveListener(EnterRemoveMode);
            gameManager.UIManager.GetWeightButton.onClick.RemoveListener(GetWeightMode);
            gameManager.UIManager.ShortestPathButton.onClick.RemoveListener(FindShortestPath);
        }

        #region Nodes

        public void AddNewNode()
        {
            Node newNode = GameObject.Instantiate<Node>(nodePrefab);
            newNode.SetGraph(this);
            newNode.SetNodeID(nodeCount++);

            nodes.Add(newNode);
        }

        public void RemoveNode()
        {
            if (nodes.Count == 0)
                return;

            Node node = nodes[nodes.Count - 1];
            nodes.Remove(node);
            GameObject.Destroy(node.gameObject);

            nodeCount--;
        }

        public void ClearNodes()
        {
            if (nodes.Count == 0)
                return;

            foreach (Node node in nodes)
            {
                if (node != null)
                    GameObject.Destroy(node.gameObject);
            }

            nodes.Clear();
            nodeCount = 0;
        }

        #endregion

        #region Modes

        public void EnterDrawMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += DrawLine;
            SelectNodes();
        }

        public void EnterRemoveMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += RemoveLine;
            SelectNodes();
        }

        public void GetWeightMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += GetWeight;
            SelectNodes();
        }

        public void FindShortestPath()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += GetShortestPath;
            SelectNodes();
        }

        #endregion

        #region Node Selection

        private void SelectNodes()
        {
            OnNodeSelect += NodeSelect1;
            isClicking = true;

            Debug.Log("Click on Node 1!");
        }

        private void NodeSelect1(int _nodeID)
        {
            selectedNode1 = nodes[_nodeID];
            OnNodeSelect -= NodeSelect1;
            OnNodeSelect += NodeSelect2;
            Debug.Log("Click on Node 2!");
        }

        private void NodeSelect2(int _nodeID)
        {
            if (nodes[_nodeID] == selectedNode1)
            {
                Debug.Log("Node 1 cannot be Node 2!");
                return;
            }

            selectedNode2 = nodes[_nodeID];
            OnNodeSelect -= NodeSelect2;

            OnNodeClickComplete?.Invoke(selectedNode1, selectedNode2);

            OnNodeClickComplete = null;

            selectedNode1 = null;
            selectedNode2 = null;

            isClicking = false;
        }

        #endregion

        private void DrawLine(Node nodeA, Node nodeB)
        {
            nodeA.AddAdjacentNode(nodeB);
            nodeB.AddAdjacentNode(nodeA);
        }

        private void RemoveLine(Node nodeA, Node nodeB)
        {
            nodeA.RemoveAdjacentNode(nodeB);
            nodeB.RemoveAdjacentNode(nodeA);
        }

        private void GetWeight(Node nodeA, Node nodeB)
        {
            float weightA = nodeA.GetWeightToNode(nodeB);
            float weightB = nodeB.GetWeightToNode(nodeA);

            if (weightA == -1 || weightB == -1)
            {
                Debug.Log("Nodes aren't connected directly");
                return;
            }

            if (weightA != weightB)
                Debug.LogWarning($"Weight discrepancies! {weightA} & {weightB}");
            else
                Debug.Log($"Weight between {nodeA.NodeID} and {nodeB.NodeID} is {weightA}");
        }

        #region Shortest Path

        private void GetShortestPath(Node nodeA, Node nodeB)
        {
            List<Node> resultPath = DijkstraShortestPath(nodeA, nodeB);

            if (resultPath == null)
                return;

            resultPath.Add(nodeA);

            resultPath.Reverse();

            HighlightPath(resultPath);
        }

        private List<Node> DijkstraShortestPath(Node nodeA, Node nodeB)
        {
            PriorityQueueFloat nodeScoreSheet = new();
            Dictionary<Node, NodeInfo> nodeParentInfos = new();

            List<Node> completedNodes = new();
            List<Node> resultPath = new();

            bool connectedToNodeB = false;

            // Initialize Score Sheet
            foreach (Node node in nodes)
            {
                NodeInfo newNodeParentInfo = new();
                newNodeParentInfo.node = node;
                newNodeParentInfo.nodeParent = null;
                nodeScoreSheet.Enqueue(float.PositiveInfinity, newNodeParentInfo);
                nodeParentInfos.Add(node, newNodeParentInfo);
            }

            // Set NodeA score to 0
            NodeInfo nodeAParentInfo = nodeParentInfos[nodeA];
            float key = nodeAParentInfo.priority;
            nodeScoreSheet.ChangePriority(key, nodeAParentInfo, 0);

            // Iterate through each node
            while (nodeScoreSheet.Count > 0)
            {
                NodeInfo currentNodeParentInfo = nodeScoreSheet.Dequeue();

                // Iterating through each adjacent node of current node
                foreach (Node adjacentNode in currentNodeParentInfo.node.AdjacentNodes)
                {
                    if (adjacentNode == nodeB)
                        connectedToNodeB = true;

                    // If node is in completedNodes then do nothing
                    if (completedNodes.Contains(adjacentNode))
                        continue;

                    // distance of current node from source
                    float nodeScore = currentNodeParentInfo.priority;

                    // distance from current node to adjacent node
                    float distanceFromNodeToAdjacent = adjacentNode.GetWeightToNode(currentNodeParentInfo.node);

                    // add both distances. This is the distance of adjacent node from source through current node
                    float newScore = nodeScore + distanceFromNodeToAdjacent;

                    // find Adjacent node's score
                    NodeInfo adjacentNodeParentInfo = nodeParentInfos[adjacentNode];
                    float adjacentNodeScore = adjacentNodeParentInfo.priority;

                    // Check if our newScore is shorter than AdjacentNode's Score
                    // if so then update it in scoreSheet
                    if (newScore < adjacentNodeScore)
                    {
                        nodeScoreSheet.ChangePriority(adjacentNodeScore, adjacentNodeParentInfo, newScore);

                        adjacentNodeParentInfo.nodeParent = currentNodeParentInfo.node;
                    }
                }

                completedNodes.Add(currentNodeParentInfo.node);
            }

            if (!connectedToNodeB)
            {
                Debug.Log("No path found!");
                return null;
            }

            Node currentNode = nodeB;

            while (currentNode != nodeA)
            {
                NodeInfo nodeParentInfo = nodeParentInfos[currentNode];

                resultPath.Add(currentNode);

                currentNode = nodeParentInfo.nodeParent;
            }

            return resultPath;
        }

        #endregion

        #region Path Highlight

        private void HighlightPath(List<Node> nodePath)
        {
            RemoveAllNodeHighlights();

            if (nodePath == null || nodePath.Count == 1)
                return;

            Node currentNode = null;
            Node nextNode = null;

            for (int i = 1; i < nodePath.Count; i++)
            {
                currentNode = nodePath[i - 1];
                nextNode = nodePath[i];
                HighlightNodeLine(currentNode, nextNode);
            }
        }

        private void RemoveAllNodeHighlights()
        {
            foreach (Node node in nodes)
            {
                node.ResetLineColors();
            }
        }

        private void HighlightNodeLine(Node nodeA, Node nodeB)
        {
            nodeA.HighlightLine(nodeB);
            nodeB.HighlightLine(nodeA);
        }

        #endregion
    }

    public class NodeInfo
    {
        public Node node;
        public Node nodeParent;
        public float priority;
    }
}
