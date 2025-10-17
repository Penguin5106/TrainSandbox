using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LinkedGraph<T>
{
    public Node<T> root { get; private set; }
    
    public int count { get; private set; }

    public LinkedGraph()
    {
        root = null;
        count = 0;
    }
    public LinkedGraph(Node<T> root)
    {
        this.root = root;
        count = 1;
    }
    
    
}

/*
 * Necessary features 
 *
 * combining 2 separate graphs
 * add to graph, random placement
 * add to graph as root (for optimised centre of graph placement)
 * restructure graph (place most "central" node as root
 * delete (must manage splitting of graphs into up to 4 unconnected graphs)
 *
 * 
 */