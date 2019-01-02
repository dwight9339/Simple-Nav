using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vertex
{
    public SerializableVector3 position;
    public List<int> adjacent = new List<int>();
    public int id;

    public Vertex(Vector3 loc, int vertId)
    {
        position = loc;
        id = vertId;
    }

    public void AddAdjacent(int u)
    {
        adjacent.Add(u);
    }

    public void RemoveAdjacent(int u){
        adjacent.Remove(u);
    }

    public bool IsAdjacent(int u)
    {
        return adjacent.Contains(u);
    }
}