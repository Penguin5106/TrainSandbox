using System;
using UnityEngine;

public class UnitTests : MonoBehaviour 
{
    private string failedTest;

    private void Start()
    {
        if (testLinkedGraph())
        {
            Debug.Log("linked graph passed all tests");
        }
        else
        {
            Debug.Log("linked graph failed a test: " + failedTest);
        }
        
    }

    public bool testLinkedGraph()
    {
        
        LinkedGraph<int> graph = new LinkedGraph<int>(new Node<int>(53));

        if (graph.count != 1)
            return false;

        Node<int> node = new Node<int>(65);
        
        if (!graph.addNode(node, graph.root))
        {
            failedTest = "add node failed";
            return false;
        }

        if (graph.count != 2)
            return false;
        
        if (!graph.containsNode(node))
            return false;
        
        if (!graph.containsNode(graph.root))
            return false;
        
        node = new Node<int>(82);
        
        graph.addRoot(node, graph.root);
        
        if (!graph.containsNode(node))
            return false;
        
        node = new Node<int>(8);
        graph.addNode(node, graph.root);
        graph.addNode(node, graph.root);
        graph.addNode(node, graph.root);
        Node<int> node2 = new Node<int>(6);
        graph.addNode(node, graph.root);
        graph.addNode(node2, graph.root);

        if (graph.deleteNode(graph.root).Count <= 1)
        {
            failedTest = "delete node failed";
            return false;
        }

        if (!graph.linkNodes(node, node2))
        {
            failedTest = "node link failed";
            return false;
        }

        //if (graph.deleteNode(node2).Count != 0)
        //    return false;

        return true;
    }
}
