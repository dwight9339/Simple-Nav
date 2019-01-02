using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMinHeap {
    Node[] Heap;
    int HeapSize = 0;

    //Constructors
    public NodeMinHeap(int size){
        Heap = new Node[size];
    }

    public NodeMinHeap(List<Node> nodes){
        Heap = new Node[nodes.Count];

        foreach (Node v in nodes){
            Insert(v);
        }
    }

    //Public methods
    public void Insert(Node v){
        HeapSize++;
        Heap[HeapSize - 1] = v;
        FloatUp(HeapSize - 1);
    }

    public Node ExtractMin(){
        Node min = Heap[0];
        Heap[0] = Heap[HeapSize - 1];
        HeapSize--;
        Heapify(0);

        return min;
    }

    void Heapify(int i){
        
        int l = Left(i);
        int r = Right(i);
        int smallest = i;

        //Check if i's children are smaller
        if (l <= HeapSize && Heap[l].f() < Heap[i].f()){
            smallest = l;
        }
        if (r <= HeapSize && Heap[r].f() < Heap[smallest].f()){
            smallest = r;
        }

        //If so swap positions and call heapify on i's new position
        if (smallest != i) {
            Swap(i, smallest);
            Heapify(smallest);
        }
    }

    public void UpdateHeap(){
        //Reinsert all nodes into heap to maintain heap property; 
        int heapSize = HeapSize;
        HeapSize = 0;
        for (int i = 0; i < heapSize; i++){
            Insert(Heap[i]);
        }
    }

    public bool IsEmpty(){
		return HeapSize == 0;
    }

	public int GetHeapSize(){
		return HeapSize;
	}

    public Node Peek(int index){
        if (IsEmpty()){
            Debug.Log("Cannot peek at empty heap");
            return null;
        }
        if (index < 0 || index >= HeapSize){
            Debug.Log("Requested index is out of bounds");
            return null;
        }

        return Heap[index];
    }

    //Helper functions
    int Parent(int i){
        return i / 2;
    }

    int Left(int i){
        return 2 * i;
    }

    int Right(int i){
        return (2 * i) + 1;
    }

    void Swap(int m, int n){
        Node temp = Heap[m];
        Heap[m] = Heap[n];
        Heap[n] = temp;
    }

    //Moves vertex i up the heap until it is on top or its parent is smaller
    void FloatUp(int index){
        if (index > HeapSize){
            Debug.Log("Index out of bounds");
            return;
        }
        int i = index;
        int p = Parent(i);
        while (i > 0 && Heap[p].f() > Heap[i].f()){
            Swap(p, i);
            i = p;
            p = Parent(i);
        }
    }
}
