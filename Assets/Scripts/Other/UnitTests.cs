using UnityEngine;

public class UnitTests
{
    public bool testLinkedGraph()
    {
        
        LinkedGraph<int> graph = new LinkedGraph<int>(new Node<int>(53));

        if (graph.count != 1)
            return false;

        Node<int> node = new Node<int>(65);
        
        if (!graph.addNode(node, graph.root))
            return false;
        
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
        graph.addNode(node, graph.root);
        graph.addNode(node, graph.root);
        
        if (graph.deleteNode(graph.root).Count <= 1)
            return false;
        
        return true;
    }
}
