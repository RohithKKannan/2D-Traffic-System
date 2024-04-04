using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarManager carManager;
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

        public void SetCarManager(CarManager _carManager)
        {
            carManager = _carManager;
        }

        private void MoveTowardsDestination()
        {
            rb.velocity = (destination - transform.position).normalized * moveSpeed;
        }

        public void BeginRandomJourney()
        {
            if (spawnNode == null)
                DecideSpawnPoint();

            DecideDestinationPoint();
            DecideShortestPath();

            StartJourney();
        }

        [ContextMenu("Toggle Highlight Path")]
        private void ToggleHighlightCurrentPath()
        {
            highlightPath = !highlightPath;

            if (!highlightPath)
                carManager.GameManager.Graph.RemoveAllNodeHighlights();
            else
                carManager.GameManager.Graph.HighlightPath(path);
        }

        private void DestinationReached()
        {
            isMoving = false;

            spawnNode = destinationNode;

            path.Clear();
            nextNode = 1;
            totalNodes = 0;
            destinationNode = null;

            BeginRandomJourney();
        }

        private void DecideSpawnPoint()
        {
            spawnNode = carManager.GameManager.Graph.GetRandomNode();

            if (spawnNode == null)
            {
                Debug.Log("No nodes to spawn in!");
                return;
            }

            transform.position = spawnNode.transform.position;
        }

        private void DecideDestinationPoint()
        {
            destinationNode = carManager.GameManager.Graph.GetRandomNode(spawnNode);
        }

        private void DecideShortestPath()
        {
            if (destinationNode == null)
            {
                Debug.Log("No destination node!");
                return;
            }

            path = carManager.GameManager.Graph.GetShortestPath(spawnNode, destinationNode);

            if (highlightPath)
                carManager.GameManager.Graph.HighlightPath(path);
        }

        private void StartJourney()
        {
            if (path == null)
            {
                Debug.Log("No path found!");
                BeginRandomJourney();
                return;
            }

            nextNode = 1;
            totalNodes = path.Count;

            GoToNextNode();
        }

        private void GoToNextNode()
        {
            isMoving = false;

            if (nextNode == totalNodes)
            {
                DestinationReached();
                return;
            }

            Node node = path[nextNode];

            if (node == null || !path[nextNode - 1].CheckIfNodeIsAdjacent(path[nextNode]))
            {
                Debug.Log("Path interrupted! Finding another way");
                FindAlternatePath();
                StartJourney();
                return;
            }

            nextNode++;

            SetDestination(node.transform.position);

            isMoving = true;
        }

        private void FindAlternatePath()
        {
            spawnNode = path[nextNode - 1];

            path.Clear();
            nextNode = 0;
            totalNodes = 0;

            DecideShortestPath();
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
