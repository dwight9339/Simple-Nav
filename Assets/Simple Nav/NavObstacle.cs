using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Used to declare a game object as navigable obstacle.
/// </summary>
[ExecuteInEditMode]
public class NavObstacle : MonoBehaviour {
    public ObstacleSettings settings = new ObstacleSettings();
    BoxCollider2D coll;
    float markerSize = 0.2f;

    void OnEnable()
    {
        coll = GetComponent<BoxCollider2D>();

        if (gameObject.layer != 8){
            gameObject.layer = 8;
        }
    }

    void OnDrawGizmosSelected(){
        // Show the location of the vertices associated with this object but only when the object is in focus
        Vector3[] locs = GetVertexLocs(0f);

        foreach (Vector3 loc in locs)
        {

            Vector3 boxSize = (Vector3.up + Vector3.right) * markerSize + Vector3.forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawCube(loc, boxSize);
            SceneView.RepaintAll();
        }
    }

    public Vector3[] GetVertexLocs(float radius){
        int cornersVisible = settings.cornersVisible;
        int arrangement = settings.arrangement;
        Vector3[] locs = new Vector3[cornersVisible];
        Vector3 center = coll.bounds.center;
        float horzFromCtr = coll.bounds.extents.x + radius;
        float vertFromCtr = coll.bounds.extents.y + radius;
        int Xcoeff = -1;
        int Ycoeff = 1;
        bool flipX = true;

        switch (arrangement){
            case 2:
                Xcoeff *= -1;
                flipX = false;
                break;
            case 3:
                Xcoeff *= -1;
                Ycoeff *= -1;
                flipX = true;
                break;
            case 4:
                Ycoeff *= -1;
                flipX = false;
                break;
            default:
                break;
        }

        for (int i = 0; i < cornersVisible; i++){
            // Skip a corner and break if corners are diagonal
            if (!settings.consecCorners && i==1){
                if (flipX)
                {
                    Xcoeff *= -1;
                }
                else
                {
                    Ycoeff *= -1;
                }
                locs[i] = new Vector3(center.x + (horzFromCtr * Xcoeff), center.y + (vertFromCtr * Ycoeff), 0);
                break;
            }

            locs[i] = new Vector3(center.x + (horzFromCtr * Xcoeff), center.y + (vertFromCtr * Ycoeff), 0);
            if (flipX){
                Xcoeff *= -1;    
            } else {
                Ycoeff *= -1;
            }
            flipX = (flipX == false);
        }

        return locs;
    }

}

[System.Serializable]
public class ObstacleSettings {
    public int cornersVisible = 4;
    public int arrangement = 1;
    public bool consecCorners = true;
}