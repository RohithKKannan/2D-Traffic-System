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

        private Node selectedNode1;
        private Node selectedNode2;

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
            OnNodeSelect += NodeSelect1;
            Debug.Log("Click on Node 1!");
        }

        public void NodeSelect1(int _nodeID)
        {
            selectedNode1 = nodes[_nodeID];
            OnNodeSelect -= NodeSelect1;
            OnNodeSelect += NodeSelect2;
            Debug.Log("Click on Node 2!");
        }

        public void NodeSelect2(int _nodeID)
        {
            if (nodes[_nodeID] == selectedNode1)
            {
                Debug.Log("Node 1 cannot be Node 2!");
                return;
            }

            selectedNode2 = nodes[_nodeID];
            OnNodeSelect -= NodeSelect2;

            DrawLine(selectedNode1, selectedNode2, 1);

            Debug.Log("Nodes connected!");

            selectedNode1 = null;
            selectedNode2 = null;
        }

        public void DrawLine(Node nodeA, Node nodeB, int weight)
        {
            nodeA.AddAdjacentNode(nodeB, weight);
            nodeB.AddAdjacentNode(nodeA, weight);
        }
    }
}
