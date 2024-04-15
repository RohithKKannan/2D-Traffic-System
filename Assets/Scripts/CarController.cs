using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private Node currentNode;

        private int carIndex;
        private int nextNodeIndex;
        private int totalNodes;
        private int retryCount;
        private int retryLimit = 5;

        private int timeToWait = 10000;

        private bool isMoving;
        private bool highlightPath;

        private List<Node> path = new();

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken cancellationToken;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            cancellationToken = cancellationTokenSource.Token;
        }

        private void OnDestroy()
        {
            if (currentNode != null && currentNode.IsOccupied)
            {
                currentNode.NodeOccupancy.Release();
                currentNode.SetNodeOccupancy(false);

                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        private void SetDestination(Vector3 _destination)
        {
            destination = _destination;
        }

        public void SetCarIndex(int _carIndex)
        {
            carIndex = _carIndex;
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
            nextNodeIndex = 1;
            totalNodes = 0;
            destinationNode = null;

            BeginRandomJourney();
        }

        private async void DecideSpawnPoint()
        {
            do
            {
                if (retryCount > retryLimit)
                    return;

                spawnNode = carManager.GameManager.Graph.GetRandomNode();
                retryCount++;
            } while (spawnNode.IsOccupied);

            retryCount = 0;

            if (spawnNode == null)
            {
                Debug.Log("No nodes to spawn in!");
                return;
            }

            transform.position = spawnNode.transform.position;

            currentNode = spawnNode;

            try
            {
                await currentNode.NodeOccupancy.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Car {carIndex} await cancelled!");
                return;
            }

            currentNode.SetNodeOccupancy(true);
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
                path = null;
                return;
            }

            path = carManager.GameManager.Graph.GetShortestPath(spawnNode, destinationNode);

            if (highlightPath)
                carManager.GameManager.Graph.HighlightPath(path);
        }

        private void StartJourney()
        {
            if (retryCount > retryLimit)
            {
                Debug.Log("Unable to find valid path! Destroying car!");
                carManager.RemoveCar(this);
                return;
            }

            if (path == null)
            {
                Debug.Log("No path found!");
                retryCount++;
                BeginRandomJourney();
                return;
            }

            retryCount = 0;

            nextNodeIndex = 1;
            totalNodes = path.Count;

            GoToNextNode(cancellationToken);
        }

        private async void GoToNextNode(CancellationToken _cancellationToken)
        {
            isMoving = false;

            if (nextNodeIndex == totalNodes)
            {
                DestinationReached();
                return;
            }

            Node nextNode = path[nextNodeIndex];

            if (nextNode == null || !currentNode.CheckIfNodeIsAdjacent(nextNode))
            {
                Debug.Log("Path interrupted! Finding another way");
                FindAlternatePath();
                StartJourney();
                return;
            }

            // additonal timer task to destroy car if waiting for node occupancy exceeds timer - maybe temporary?
            var timeoutTask = Task.Delay(timeToWait, cancellationToken);

            try
            {
                var completedTask = await Task.WhenAny(nextNode.NodeOccupancy.WaitAsync(cancellationToken), timeoutTask);

                // completed task is the first task that has returned from WhenAny
                if (completedTask == timeoutTask)
                {
                    Debug.Log($"Car stuck for a long time. Destroying car {carIndex}!");
                    carManager.RemoveCar(this);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Car {carIndex} await cancelled!");
                return;
            }

            nextNode.SetNodeOccupancy(true);

            currentNode.NodeOccupancy.Release();
            currentNode.SetNodeOccupancy(false);

            nextNodeIndex++;

            SetDestination(nextNode.transform.position);

            isMoving = true;

            currentNode = nextNode;
        }

        private void FindAlternatePath()
        {
            spawnNode = currentNode;

            path.Clear();
            totalNodes = 0;

            DecideShortestPath();

            if (highlightPath)
                carManager.GameManager.Graph.HighlightPath(path);
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
                GoToNextNode(cancellationToken);
            }
        }
    }
}
