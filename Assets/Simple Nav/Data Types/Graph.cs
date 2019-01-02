using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : ScriptableObject {
    [HideInInspector]public Vertex[] graph;

    public static Graph CreateInstance(Vertex[] graph){
        Graph saveGraph = ScriptableObject.CreateInstance<Graph>();
        saveGraph.graph = new Vertex[graph.Length];
        for (int i = 0; i < graph.Length; i++)
        {
            saveGraph.graph[i] = graph[i];
        }

        return saveGraph;
    }
}
