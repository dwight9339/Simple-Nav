using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavObstacle))]
public class NavObstacleEditor : Editor {
    //Add custom icon to editor
    GUIContent[] 
        opts = new GUIContent[] { new GUIContent("1"), new GUIContent("2"), new GUIContent("3"), new GUIContent("4") },
        nonConsecCornersOpts = new GUIContent[] { new GUIContent("1"), new GUIContent("2") };
    int cornersSelectedIndex, arrangementSelectedIndex;
    GUIContent cornersContent = new GUIContent(), arrngmtContent = new GUIContent(), consecCornersContent = new GUIContent();
    NavObstacle obstacle;

    private void OnEnable()
    {
        obstacle = (target as NavObstacle);

        cornersContent.text = "Corners Visible";
        cornersContent.tooltip = "The number of corners of the box to be used in making nav graphs";

        arrngmtContent.text = "Arrangement";
        arrngmtContent.tooltip = "The arrangement of the corners used to make nav graphs";

        consecCornersContent.text = "Consecutive Corners";
        consecCornersContent.tooltip = "Are the corners consecutive or diagonal?";
    }


    // Draw GUI in inspector
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        cornersSelectedIndex = obstacle.settings.cornersVisible - 1;
        arrangementSelectedIndex = obstacle.settings.arrangement - 1;

        // Display corners selected popup menu
        EditorGUI.BeginChangeCheck();
        cornersSelectedIndex = EditorGUILayout.Popup(cornersContent, cornersSelectedIndex, opts);
        if (EditorGUI.EndChangeCheck()){
            obstacle.settings.cornersVisible = cornersSelectedIndex + 1;
            obstacle.settings.arrangement = 1;
            arrangementSelectedIndex = 0;
            Undo.RecordObject(obstacle, "Changed corners visible");
        }

        // If two corners are visible, display consecutive corners toggle control
        if (cornersSelectedIndex == 1){
            EditorGUI.BeginChangeCheck();
            obstacle.settings.consecCorners = EditorGUILayout.Toggle(consecCornersContent, obstacle.settings.consecCorners);
            if (EditorGUI.EndChangeCheck()){
                obstacle.settings.arrangement = 1;
                arrangementSelectedIndex = 0;
                Undo.RecordObject(obstacle, "Toggled consecutive corners");
            }
        }

        // Display arrangement popup menu
        EditorGUI.BeginChangeCheck();
        // Change the available options based on corners visible and consecutive corner settings
        if (cornersSelectedIndex == 1 && !obstacle.settings.consecCorners){
            arrangementSelectedIndex = EditorGUILayout.Popup(arrngmtContent, arrangementSelectedIndex, nonConsecCornersOpts);
        } else if (cornersSelectedIndex != 3){
            arrangementSelectedIndex = EditorGUILayout.Popup(arrngmtContent, arrangementSelectedIndex, opts);
        }

        if (EditorGUI.EndChangeCheck()){
            Undo.RecordObject(obstacle, "Changed arrangement");
            obstacle.settings.arrangement = arrangementSelectedIndex + 1;
        }


    }

}
