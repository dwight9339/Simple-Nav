using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor window class used to create graph assets
/// </summary>
public class SimpleGraph : EditorWindow
{
    public float agentRadius = 0;
    string graphName = "New Graph", graphsFolderPath, currentScene;
    GUIContent nameContent = new GUIContent(), radiusContent = new GUIContent();


    [MenuItem("Window/Simple Graph")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SimpleGraph window = (SimpleGraph)EditorWindow.GetWindow(typeof(SimpleGraph));
        window.Show();
    }

    void OnEnable()
    {
        graphsFolderPath = Application.dataPath + "/Graphs";
        currentScene = "/" + SceneManager.GetActiveScene().name;

        nameContent.text = "Graph Name";
        nameContent.tooltip = "Name for the graph file";

        radiusContent.text = "Agent Radius";
        radiusContent.tooltip = "The radius of the circle collider of the agent to use the graph";
    }

    void OnGUI()
    {
        graphName = EditorGUILayout.TextField(nameContent, graphName);
        agentRadius = EditorGUILayout.FloatField(radiusContent, agentRadius);

        if (agentRadius < 0)
            EditorGUILayout.HelpBox("Agent radius cannot be negative", MessageType.Error);


        if (GUILayout.Button("Build Graph") && agentRadius >= 0){
            CreateGraph();

        }
    }

    //Graph Ops
    void CreateGraph(){

        // Make sure name field isn't empty
        if (string.IsNullOrEmpty(graphName) || CheckIsWhiteSpace(graphName)){
            EditorUtility.DisplayDialog("Empty Graph Name", "Graph name cannot be empty", "Ok");
            return;
        }

        //// Check if graph of same name exists
        if (File.Exists(Application.dataPath + "/Graphs" + currentScene + "/" + graphName + ".asset"))
        {
            if (!EditorUtility.DisplayDialog("Overwrite Graph?",
                                            "A graph with this name already exists. Ok to overwrite?",
                                            "Overwrite",
                                             "Cancel"))
            {
                return;
            }
        }

        Debug.Log("Creating Graph");
        int size = 0;
        NavObstacle[] obstacles = FindObjectsOfType<NavObstacle>();
        int i = 0;

        //Get number of vertices in graph
        foreach (NavObstacle obst in obstacles){
            size += obst.settings.cornersVisible;
        }
        Vertex[] graph = new Vertex[size];

        //Get vertex locations from NavObstacle scripts
        foreach (NavObstacle obst in obstacles){
            Vector3[] locs = obst.GetVertexLocs(agentRadius);

            foreach (Vector3 loc in locs){
                graph[i] = new Vertex(loc, i);
                i++;
            }
        }

        //Get adjacency information
        foreach (Vertex v in graph){
            GetAdjacent(v, graph);
        }

        // Create graphs folder in assets folder if none currently exists
        Directory.CreateDirectory(graphsFolderPath);
        Directory.CreateDirectory(graphsFolderPath + currentScene);
        SaveGraph(graph);
        //UpdateGraphList();
    }

    void GetAdjacent(Vertex u, Vertex[] graph){
        Vector2 uPos = new Vector2(u.position.x, u.position.y);
        Vector2 vPos;

        foreach (Vertex v in graph)
        {
            if (u == v || v.IsAdjacent(u.id))
            {
                continue;
            }

            vPos = new Vector2(v.position.x, v.position.y);
            Vector2 direction = vPos - uPos;
            float distance = direction.magnitude;
            direction.Normalize();
            RaycastHit2D obstacle = Physics2D.CircleCast(uPos, agentRadius, direction, distance, 1 << 8);

            if (!obstacle)
            {
                u.AddAdjacent(v.id);
                v.AddAdjacent(u.id);
            }
        }
    }

    //File ops
    void SaveGraph(Vertex[] graph){
        Graph save = Graph.CreateInstance(graph);
        AssetDatabase.CreateAsset(save, "Assets/Graphs" + currentScene + "/" + graphName + ".asset");
    }

    void SceneOpenedCallback(Scene scene, OpenSceneMode mode){
        currentScene = "/" + scene.name;
    }

    bool CheckIsWhiteSpace(string str){
        bool isWS = true;
        foreach (char c in str){
            if (c != ' ')
                isWS = false;
        }

        return isWS;
    }
}