using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TS
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Node nodePrefab;

        private List<Node> nodes = new();
        private int nodeCount;

        public UnityAction<int> OnNodeSelect;
        public UnityAction<Node, Node> OnNodeClickComplete;

        private Node selectedNode1;
        private Node selectedNode2;

        private bool isClicking;

        [ContextMenu("Add New Node")]
        public void AddNewNode()
        {
            Node newNode = GameObject.Instantiate<Node>(nodePrefab);
            newNode.SetGraph(this);
            newNode.SetNodeID(nodeCount++);

            nodes.Add(newNode);
        }

        [ContextMenu("Clear All Nodes")]
        public void ClearNodes()
        {
            foreach (Node node in nodes)
            {
                if (node != null)
                    GameObject.DestroyImmediate(node.gameObject);
            }

            nodes.Clear();
            nodeCount = 0;
        }

        [ContextMenu("Draw Mode")]
        public void EnterDrawMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += DrawLine;
            SelectNodes();
        }

        [ContextMenu("Remove Mode")]
        public void EnterRemoveMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += RemoveLine;
            SelectNodes();
        }

        [ContextMenu("Print Weight")]
        public void GetWeightMode()
        {
            if (isClicking)
                return;

            OnNodeClickComplete += GetWeight;
            SelectNodes();
        }

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

        public void DrawLine(Node nodeA, Node nodeB)
        {
            nodeA.AddAdjacentNode(nodeB);
            nodeB.AddAdjacentNode(nodeA);
        }

        public void RemoveLine(Node nodeA, Node nodeB)
        {
            nodeA.RemoveAdjacentNode(nodeB);
            nodeB.RemoveAdjacentNode(nodeA);
        }

        public void GetWeight(Node nodeA, Node nodeB)
        {
            float weightA = nodeA.GetWeightToNode(nodeB);
            float weightB = nodeB.GetWeightToNode(nodeA);

            if (weightA != weightB)
                Debug.LogWarning($"Weight discrepancies! {weightA} & {weightB}");
            else
                Debug.Log($"Weight between {nodeA.NodeID} and {nodeB.NodeID} is {weightA}");
        }
    }
}
