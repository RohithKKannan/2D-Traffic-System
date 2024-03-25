using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TS
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private TMP_Text nodeLabel;
        [SerializeField] private LineRenderer lineRenderer;

        private Graph graph;
        private int nodeID;

        private List<Node> adjacentNodes = new();
        private List<int> weights = new();

        private Vector3 mousePosition;

        public void SetGraph(Graph _graph)
        {
            graph = _graph;
        }

        public void SetNodeID(int _nodeID)
        {
            nodeID = _nodeID;
            nodeLabel.text = nodeID.ToString();
        }

        public void AddAdjacentNode(Node _node, int _weight)
        {
            if (adjacentNodes.Contains(_node))
            {
                Debug.Log("Node already connected!");
                return;
            }

            adjacentNodes.Add(_node);
            weights.Add(_weight);

            DrawLineToNode(_node);
        }

        private void DrawLineToNode(Node _node2)
        {
            lineRenderer.positionCount++;

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, _node2.transform.position);

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material.color = Color.yellow;
        }

        public int GetWeightToNode(Node _node)
        {
            if (!adjacentNodes.Contains(_node))
                return -1;

            return weights[adjacentNodes.IndexOf(_node)];
        }

        [ContextMenu("Click Node")]
        private void OnMouseUp()
        {
            graph.OnNodeSelect?.Invoke(nodeID);
        }

        private void Update()
        {
            lineRenderer.SetPosition(0, this.transform.position);
            for (int i = 1; i <= adjacentNodes.Count; i++)
            {
                lineRenderer.SetPosition(i, adjacentNodes[i - 1].transform.position);
            }
        }
    }
}
