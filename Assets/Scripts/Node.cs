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
        [SerializeField] private Gradient redGradient;
        [SerializeField] private Gradient yellowGradient;

        private Graph graph;

        private int nodeID;
        private bool isDragging;
        private Vector3 startPosition;
        private Vector3 dragOffset;

        private List<Node> adjacentNodes = new();
        private List<float> weights = new();
        private List<LineRenderer> lineRenderers = new();

        private Vector3 mousePosition;

        public int NodeID => nodeID;
        public List<Node> AdjacentNodes => adjacentNodes;
        public List<float> Weights => weights;

        private void OnDestroy()
        {
            while (adjacentNodes.Count > 0)
            {
                adjacentNodes[adjacentNodes.Count - 1].RemoveAdjacentNode(this);
                RemoveAdjacentNode(adjacentNodes[adjacentNodes.Count - 1]);
            }
        }

        public void SetGraph(Graph _graph)
        {
            graph = _graph;
        }

        public void SetNodeID(int _nodeID)
        {
            nodeID = _nodeID;
            nodeLabel.text = nodeID.ToString();
        }

        public void AddAdjacentNode(Node _node)
        {
            if (adjacentNodes.Contains(_node))
            {
                Debug.Log("Node already connected!");
                return;
            }

            float _weight = Vector3.Distance(this.transform.position, _node.transform.position);

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
            newLineRenderer.colorGradient = yellowGradient;

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

        public void HighlightLine(Node _node2)
        {
            if (!adjacentNodes.Contains(_node2))
            {
                Debug.Log("Adjacent node not found!");
                return;
            }

            int index = adjacentNodes.IndexOf(_node2);

            LineRenderer lineRenderer = lineRenderers[index];

            lineRenderer.colorGradient = redGradient;
        }

        public void ResetLineColors()
        {
            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                lineRenderer.colorGradient = yellowGradient;
            }
        }

        public float GetWeightToNode(Node _node)
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

            for (int i = 0; i < adjacentNodes.Count; i++)
            {
                weights[i] = Vector3.Distance(this.transform.position, adjacentNodes[i].transform.position);
            }
        }
    }
}
