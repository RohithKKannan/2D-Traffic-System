using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace TS
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private TMP_Text nodeLabel;
        [SerializeField] private LineRenderer linePrefab;
        [SerializeField] private Transform lineParent;

        private Graph graph;

        private int nodeID;
        private bool isDragging;
        private Vector3 startPosition;
        private Vector3 dragOffset;

        private List<Node> adjacentNodes = new();
        private List<int> weights = new();
        private List<LineRenderer> lineRenderers = new();

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

            lineRenderers.Add(DrawLineToNode(_node));
        }

        private LineRenderer DrawLineToNode(Node _node2)
        {
            LineRenderer newLineRenderer = GameObject.Instantiate<LineRenderer>(linePrefab, lineParent);

            newLineRenderer.positionCount = 2;

            newLineRenderer.SetPosition(0, this.transform.position);
            newLineRenderer.SetPosition(1, _node2.transform.position);

            newLineRenderer.startWidth = 0.1f;
            newLineRenderer.endWidth = 0.1f;
            newLineRenderer.material.color = Color.yellow;

            return newLineRenderer;
        }

        public void RemoveAdjacentNode(Node _node2)
        {
            if (!adjacentNodes.Contains(_node2))
            {
                Debug.Log("Adjacent node not found!");
                return;
            }

            int index = adjacentNodes.IndexOf(_node2);

            LineRenderer lineRenderer = lineRenderers[index];
            lineRenderers.Remove(lineRenderer);
            GameObject.Destroy(lineRenderer.gameObject);

            adjacentNodes.Remove(_node2);
            weights.RemoveAt(index);
        }

        public int GetWeightToNode(Node _node)
        {
            if (!adjacentNodes.Contains(_node))
                return -1;

            return weights[adjacentNodes.IndexOf(_node)];
        }

        private void OnMouseDown()
        {
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startPosition = transform.position;
            isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (isDragging)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + dragOffset;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
        }

        [ContextMenu("Click Node")]
        private void OnMouseUp()
        {
            graph.OnNodeSelect?.Invoke(nodeID);

            isDragging = false;
        }

        private void Update()
        {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].SetPosition(0, this.transform.position);
                lineRenderers[i].SetPosition(1, adjacentNodes[i].transform.position);
            }
        }
    }
}
