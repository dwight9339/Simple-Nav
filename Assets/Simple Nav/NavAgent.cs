using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to control nav agents
/// </summary>
public class NavAgent : MonoBehaviour
{
    // Private Attributes
    Vector3 destination;
    Vector3 nextCheckpoint;
    Stack<Vector3> path;
    bool destinationChanged = false;
    float agentRadius;

    // Public Fields

    /// <summary>
    /// Graph asset file used by the nav agent
    /// </summary>
    public Graph graph;

    /// <summary>
    /// The speed of the nav agent
    /// </summary>
    public float speed = 0;

    /// <summary>
    /// How far away from the destination the agent will stop
    /// </summary>
    public float stoppingDistance = 0;

    /// <summary>
    /// Normalized vector representing the agent's current direction of travel. 
    /// Set to zero vector if agent is stopped.
    /// </summary>
    public Vector3 currentDirection { get; private set; }

    /// <summary>
    /// Should agent movement be handled automatically?
    /// </summary>
    public bool updatePosition = true;

    /// <summary>
    /// Does the agent have a path to follow?
    /// </summary>
    public bool pathExists = false;

    /// <summary>
    /// Is the agent currently moving towards its destination?
    /// </summary>
    public bool enRoute = false;

    // Graph Preview Variables

    /// <summary>
    /// Draw a preview of the graph in the scene view?
    /// </summary>
    public bool drawGraph = true;

    /// <summary>
    /// The color of the graph preview
    /// </summary>
    public Color previewColor = Color.white;

    void Awake()
    {
        if (!graph){
            Debug.LogError("No graph added!");
        }
        destination = transform.position;
        agentRadius = GetComponent<CircleCollider2D>().radius;
        currentDirection = Vector3.zero;
    }

    void Update()
    {
        if (graph){
            if (destinationChanged) {
                path = GetPath(destination);
                if (path.Count > 0) {
                    nextCheckpoint = path.Pop();
                    pathExists = true;
                    enRoute = true;
                } 
                destinationChanged = false;
            }

            if (transform.position != destination)
            {
                if (transform.position == nextCheckpoint)
                {
                    nextCheckpoint = path.Pop();
                }
                if (updatePosition && Vector3.Distance(transform.position, destination) > stoppingDistance)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextCheckpoint, speed * Time.deltaTime);
                }
                currentDirection = (nextCheckpoint - transform.position).normalized;
            }
            else
            {
                currentDirection = Vector3.zero;
                enRoute = false;
            }
        }
    }

    //Public methods

    /// <summary>
    /// Sets a new destination for the nav agent
    /// </summary>
    /// <param name="destination">The desired destination</param>
    public void SetDestination(Vector3 destination)
    {
        if (this.destination != destination)
        {
            this.destination = destination;
            destinationChanged = true;
        }
    }

    /// <summary>
    /// Checks if a valid path to the given destination can be found
    /// </summary>
    /// <returns><c>true</c>, if path was found, <c>false</c> otherwise </returns>
    /// <param name="destination">Destination to check</param>
    public bool CheckPath(Vector3 destination){
        return !(GetPath(destination).Count == 0);
    }

    // Private Methods
    Stack<Vector3> GetPath(Vector3 dest)
    {
        Stack<Vector3> newPath = new Stack<Vector3>();

        //Perform circle cast to see if destination is within sight
        Vector2 uPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = new Vector2(dest.x, dest.y) - uPos;

        RaycastHit2D obstacle = Physics2D.CircleCast(uPos, agentRadius, direction.normalized, direction.magnitude, 1 << 8);
        if (!obstacle)
        {
            newPath = new Stack<Vector3>();
            newPath.Push(dest);
            return newPath;
        }

        //Perform A* to get shortest path
        Vertex uVert = new Vertex(transform.position, graph.graph.Length);
        Vertex vVert = new Vertex(dest, graph.graph.Length + 1);
        GetAdjacent(uVert);
        GetAdjacent(vVert);

        List<Node> nodes = new List<Node>();

        foreach (Vertex p in graph.graph)
        {
            nodes.Add(new Node(p, Vector3.Distance(p.position, dest)));
        }
        Node u = new Node(uVert, 0);
        Node v = new Node(vVert, 0);
        u.distance = 0;
        nodes.Add(u);
        nodes.Add(v);

        foreach (Node p in nodes)
        {
            foreach (int q in p.vertex.adjacent)
            {
                p.adjacent.Add(nodes[q]);
            }
        }

        NodeMinHeap Q = new NodeMinHeap(nodes);

        // Find shortest path
        Node m = null;

        while (!Q.IsEmpty())
        {
            m = Q.ExtractMin();

            foreach (Node n in m.adjacent)
            {
                if (n == v){
                    v.parent = m;
                    goto StopSearch;
                }
                RelaxEdge(m, n);
            }
            Q.UpdateHeap();
        }

        StopSearch:

        RemoveAdjacency(uVert);
        RemoveAdjacency(vVert);

        //Extract Path
        Node next = v;
        while (next.parent != null)
        {
            newPath.Push(next.vertex.position);
            next = next.parent;
        }

        return newPath;
    }

    float GetWeight(Vertex u, Vertex vertex)
    {
        return Vector3.Distance(u.position, vertex.position);
    }

    void RelaxEdge(Node u, Node v)
    {
        float w = GetWeight(u.vertex, v.vertex);
        if (v.distance > u.distance + w)
        {
            v.distance = u.distance + w;
            v.parent = u;
        }
    }

    void GetAdjacent(Vertex u)
    {
        Vector2 uPos = new Vector2(u.position.x, u.position.y);
        Vector2 vPos;

        foreach (Vertex v in graph.graph)
        {
            if (u == v || v.IsAdjacent(u.id))
            {
                continue;
            }

            vPos = new Vector2(v.position.x, v.position.y);
            Vector2 direction = vPos - uPos;
            float distance = direction.magnitude;
            direction.Normalize();
            RaycastHit2D obstacle = Physics2D.CircleCast(uPos + (direction * agentRadius), agentRadius, direction, distance, 1 << 8);

            if (!obstacle)
            {
                u.AddAdjacent(v.id);
                v.AddAdjacent(u.id);
            }
        }
    }

    void RemoveAdjacency(Vertex u)
    {
        foreach (int v in u.adjacent)
        {
            graph.graph[v].RemoveAdjacent(u.id);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (graph && drawGraph){
            Vector3 boxSize = (Vector3.up + Vector3.right + Vector3.forward) * 0.2f;

            foreach (Vertex u in graph.graph)
            {
                foreach (int v in u.adjacent)
                {
                    Gizmos.color = previewColor;
                    Gizmos.DrawLine(u.position, graph.graph[v].position);
                    Gizmos.DrawCube(u.position, boxSize);
                }
            }
        }
    }
}