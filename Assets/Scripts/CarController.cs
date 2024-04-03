using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float minDistance = 0.05f;

        private Rigidbody2D rb;

        private Vector3 destination = new();
        private Node spawnNode;
        private Node destinationNode;

        private int nextNode;
        private int totalNodes;

        private bool isMoving;
        private bool highlightPath;

        [SerializeField] private List<Node> path = new();

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void SetDestination(Vector3 _destination)
        {
            destination = _destination;
        }

        private void MoveTowardsDestination()
        {
            rb.velocity = (destination - transform.position).normalized * moveSpeed;
        }

        [ContextMenu("Begin Journey!")]
        private void BeginRandomJourney()
        {
            if (!gameManager.Graph.IsGraphReady())
                return;

            if (spawnNode == null)
                DecideSpawnPoint();

            DecideDestinationPoint();
            DecideShortestPath();

            if (highlightPath)
                gameManager.Graph.HighlightPath(path);

            StartJourney();
        }

        [ContextMenu("Toggle Highlight Path")]
        private void ToggleHighlightCurrentPath()
        {
            highlightPath = !highlightPath;

            if (!highlightPath)
                gameManager.Graph.RemoveAllNodeHighlights();
            else
                gameManager.Graph.HighlightPath(path);
        }

        private void DestinationReached()
        {
            isMoving = false;

            spawnNode = destinationNode;

            path.Clear();
            nextNode = 0;
            totalNodes = 0;
            destinationNode = null;

            BeginRandomJourney();
        }

        private void DecideSpawnPoint()
        {
            spawnNode = gameManager.Graph.GetRandomNode();

            if (spawnNode == null)
            {
                Debug.Log("No nodes to spawn in!");
                return;
            }

            transform.position = spawnNode.transform.position;
        }

        private void DecideDestinationPoint()
        {
            destinationNode = gameManager.Graph.GetRandomNode(spawnNode);

            if (destinationNode == null)
                Debug.Log("No destination node!");
        }

        private void DecideShortestPath()
        {
            if (destinationNode == null)
            {
                Debug.Log("No destination node!");
                return;
            }

            path = gameManager.Graph.GetShortestPath(spawnNode, destinationNode);

            if (path == null)
                Debug.Log("No path found!");
        }

        private void StartJourney()
        {
            if (path == null)
            {
                Debug.Log("No path found!");
                return;
            }

            nextNode = 1;
            totalNodes = path.Count;

            isMoving = true;

            GoToNextNode();
        }

        private void GoToNextNode()
        {
            if (nextNode == totalNodes)
            {
                DestinationReached();
                return;
            }

            Node node = path[nextNode++];

            if (node == null)
            {
                Debug.Log("Path interrupted!");

                spawnNode = null;
                destinationNode = null;
                DestinationReached();

                return;
            }

            SetDestination(node.transform.position);
        }

        private void FixedUpdate()
        {
            if (!isMoving)
                return;

            if (Vector3.Distance(transform.position, destination) > minDistance)
                MoveTowardsDestination();
            else
            {
                rb.velocity = Vector2.zero;
                GoToNextNode();
            }
        }
    }
}
