using System.Collections.Generic;
using UnityEngine;

public class Node <T>
{
    public T data { get; set; }
    
    public Dictionary<int, Node<T>> links { get; internal set; }
    
    public Node(T data)
    {
        this.data = data;
        links = new Dictionary<int, Node<T>>();
    }
}
