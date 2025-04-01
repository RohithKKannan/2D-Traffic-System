# 2D Traffic System

A Unity-based traffic simulation inspired by **Cities: Skylines**, utilizing **graph-based pathfinding** to simulate vehicle movement and road networks.

### Features

- **Graph-Based Road System** – Roads are represented as **nodes (intersections)** and **edges (roads)** with weighted distances.
- **Pathfinding with Dijkstra's Algorithm** – Vehicles find the shortest route between nodes.
- **Vehicle AI** – Cars spawn at a node, choose a destination, and navigate the graph.
- **Queueing System & Deadlock Prevention** – Cars **wait for free nodes** before moving, using **semaphores** to avoid conflicts.
- **Directed Graph (Di-Graph) Support** – Simulates **one-way roads** with directed edges.

### How It Works

1. Nodes represent **intersections** and edges represent **roads**.
2. Vehicles spawn at random nodes and find the **shortest path** to a destination.
3. A **queueing system** ensures cars move without collisions.
4. The system uses a **directed graph**, meaning roads are one-way unless explicitly connected both ways.

### Future Improvements

- **Grid-Based Road Building System** – Automate road placement with smart intersection handling.
- **Traffic Lights & Lane Systems** – Improve traffic flow logic.
- **Dynamic Congestion Weights** – Adjust pathfinding based on real-time traffic conditions.
