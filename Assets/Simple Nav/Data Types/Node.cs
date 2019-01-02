using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Vertex vertex { set; get; }
    public List<Node> adjacent = new List<Node>();
    public float distance { set; get; }
    public Node parent { set; get; }
    public float h { set; get; }

    public Node(Vertex v, float h){
        vertex = v;
        distance = Mathf.Infinity;
        parent = null;
        this.h = h;
    }

    public float f(){
        return distance + h;
    }
}
