using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(NavAgent))]
[CanEditMultipleObjects]
public class NavAgentEditor : Editor {
    GUIContent
    graphLabel = new GUIContent(),
    speedLabel = new GUIContent(),
    stopDistLabel = new GUIContent(),
    updatePosLabel = new GUIContent(),
    drawGraphLabel = new GUIContent(),
    graphColorLabel = new GUIContent();

    void OnEnable()
    {
        graphLabel.text = "Graph";
        graphLabel.tooltip = "Graph asset file used by nav agent";

        speedLabel.text = "Speed";
        speedLabel.tooltip = "Speed of nav mesh agent if auto updating position";

        stopDistLabel.text = "Stopping Distance";
        stopDistLabel.tooltip = "How far away from the destination the agent will stop";

        updatePosLabel.text = "Auto-Update Position";
        updatePosLabel.tooltip = "Automatically move the nav agent to its specified destination?";

        drawGraphLabel.text = "Draw Graph";
        drawGraphLabel.tooltip = "Show the attached graph in scene view?";

        graphColorLabel.text = "Preview Color";
        graphColorLabel.tooltip = "The color of the graph in scene view when visible";
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NavAgent agent = (target as NavAgent);

        // Graph
        EditorGUI.BeginChangeCheck();
        agent.graph = (Graph)EditorGUILayout.ObjectField(graphLabel, agent.graph, typeof(Graph), false);
        if (EditorGUI.EndChangeCheck()){
            Undo.RecordObject(agent, "Changed the graph object");
        }

        if (!agent.graph)
        {
            EditorGUILayout.HelpBox("Warning: Graph file required", MessageType.Warning, true);
        }

        // Auto-move options
        EditorGUILayout.Space();
        GUIStyle moveStyle = new GUIStyle();
        moveStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Auto Move:", moveStyle);

        EditorGUI.BeginChangeCheck();
        agent.updatePosition = EditorGUILayout.Toggle(updatePosLabel, agent.updatePosition);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(agent, "Toggled update position option");
        }

        EditorGUI.BeginDisabledGroup(!agent.updatePosition);
        EditorGUI.BeginChangeCheck();
        agent.speed = EditorGUILayout.Slider(speedLabel, agent.speed, 0, 20);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(agent, "Changed agent speed");
        }

        EditorGUI.BeginChangeCheck();
        agent.stoppingDistance = EditorGUILayout.DelayedFloatField(stopDistLabel, agent.stoppingDistance);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(agent, "Changed the agent's stopping distance");
        }
        EditorGUI.EndDisabledGroup();

        // Graph Preview
        EditorGUILayout.Space();
        GUIStyle previewStyle = new GUIStyle();
        previewStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Graph Preview:", previewStyle);

        EditorGUI.BeginChangeCheck();
        agent.drawGraph = EditorGUILayout.Toggle(drawGraphLabel, agent.drawGraph);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(agent, "Toggled draw graph option");
        }

        EditorGUI.BeginDisabledGroup(!agent.drawGraph);
            EditorGUI.BeginChangeCheck();
        agent.previewColor = EditorGUILayout.ColorField(graphColorLabel, agent.previewColor);
            if (EditorGUI.EndChangeCheck()){
                Undo.RecordObject(agent, "Changed Graph Preview Color");
            }
        EditorGUI.EndDisabledGroup();
    }
}