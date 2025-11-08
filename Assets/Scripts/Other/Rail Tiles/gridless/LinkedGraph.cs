using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
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
        countNodes();
    }

    public bool containsNode(Node<T> toFind)
    {
        int i = 0;
        Node<T> tocheck = root;
        
        if (toFind == tocheck)
            return true;

        Dictionary<int, Node<T>> seen = new Dictionary<int, Node<T>>();
        seen.Add(i, tocheck);
        Dictionary<int, Node<T>> explored = new Dictionary<int, Node<T>>();

        while (i < count)
        {
            i++;
            foreach(Node<T> node in tocheck.links.Values)
            {
                if (tocheck.links.ContainsValue(toFind))
                    return true;

                if (!seen.ContainsValue(node))
                    seen.Add(seen.Count, node);
            }

            explored.Add(explored.Count, tocheck);

            foreach(Node<T> node in seen.Values)
            {
                if (!explored.ContainsValue(node))
                {
                    tocheck = node;
                    continue;
                }
            }

        }

        return false;
    }

    public void countNodes()
    {
        Node<T> tocheck = root;

        Dictionary<int, Node<T>> seen = new Dictionary<int, Node<T>>();
        seen.Add(0, tocheck);
        Dictionary<int, Node<T>> explored = new Dictionary<int, Node<T>>();

        while (seen.Count != explored.Count )
        {

            foreach (Node<T> node in tocheck.links.Values)
            {

                if (!seen.ContainsValue(node))
                    seen.Add(seen.Count, node);
            }

            explored.Add(explored.Count, tocheck);

            foreach (Node<T> node in seen.Values)
            {
                if (!explored.ContainsValue(node))
                {
                    tocheck = node;
                    continue;
                }
            }

        }

        count = seen.Count;

    }
    
    public bool addNode(Node<T> node, Node<T> link)
    {
        if (link == null || node == null || !containsNode(link) || containsNode(node)) 
        {
            return false;
        }

        node.links.Add(node.links.Count, link);
        link.links.Add(link.links.Count, node);

        count++;

        return true;
    }

    public void addRoot(Node<T> node, Node<T> link)
    {
        if (containsNode(node))
        {
            root = node;

            if (link == null || root.links.ContainsValue(link))
                return;

            root.links.Add(root.links.Count, link);
            return;
        }

        if (!containsNode(link))
            return;

        addNode(node, link);
        
        root = node;
        
    }

    public Dictionary<int, LinkedGraph<T>>  deleteNode(Node<T> node)
    {

        Dictionary<int, LinkedGraph<T>> LooseGraphs = new Dictionary<int, LinkedGraph<T>>();

        if (node == null || !containsNode(node)) return LooseGraphs;

        if (node.links.Count == 0 && node == root)
        { 
            root = null;
            count = 0;
            return LooseGraphs;
        }

        if (node.links.Count == 1 && node == root)
        {
            foreach (Node<T> link in node.links.Values)
            {
                if (link != null)
                { 
                    root = link;
                    count--;
                    return LooseGraphs;
                }
            }
        }

        if (node.links.Count == 1)
        {
            foreach (Node<T> link in  node.links.Values)
            {
                foreach(KeyValuePair<int, Node<T>> linkPair in link.links)
                {
                    if (linkPair.Value == node)
                    {
                        link.links.Remove(linkPair.Key);
                    }
                }
            }
            count--;
            return LooseGraphs;
        }


        if (node.links.Count > 1 &&  node == root)
        {
            Dictionary<int, Node<T>> severedLinks = node.links;

            foreach (Node<T> link in severedLinks.Values)
            {
                foreach (KeyValuePair<int, Node<T>> linkPair in link.links)
                {
                    if (linkPair.Value == node)
                    {
                        link.links.Remove(linkPair.Key);
                        break;
                    }
                    
                }
            }

            foreach(Node<T> link in severedLinks.Values)
            {
                LooseGraphs.Add(LooseGraphs.Count, new LinkedGraph<T>(link));
            }

            int i = 0;

            while (i < LooseGraphs.Count-1)
            {

                int j = 1;
                LinkedGraph<T> primaryGraph = null;
                LooseGraphs.TryGetValue(i, out primaryGraph);

                while (j < LooseGraphs.Count)
                {
                    LinkedGraph<T> comparison = null;
                    LooseGraphs.TryGetValue(j, out comparison);

                    if (primaryGraph.containsNode(comparison.root))
                    {
                        LooseGraphs.Remove(j);
                    }

                    j++;
                }

                root = primaryGraph.root;
                i++;
            }

            return LooseGraphs;
        }

        if (node.links.Count > 1)
        {
            Dictionary<int, Node<T>> severedLinks = node.links;

            foreach (Node<T> link in severedLinks.Values)
            {
                foreach (KeyValuePair<int, Node<T>> linkPair in link.links)
                {
                    if (linkPair.Value == node)
                        link.links.Remove(linkPair.Key);
                }
            }

            foreach (Node<T> link in severedLinks.Values)
            {
                LooseGraphs.Add(LooseGraphs.Count, new LinkedGraph<T>(link));
            }

            int i = 0;

            while (i < LooseGraphs.Count - 1)
            {

                int j = 1;
                LinkedGraph<T> primaryGraph = null;
                LooseGraphs.TryGetValue(i, out primaryGraph);

                if (primaryGraph.containsNode(root))
                {
                    LooseGraphs.Remove(i);
                    i++;

                    continue; 
                }

                while (j < LooseGraphs.Count)
                {
                    LinkedGraph<T> comparison = null;
                    LooseGraphs.TryGetValue(j, out comparison);

                    if (primaryGraph.containsNode(comparison.root))
                    {
                        LooseGraphs.Remove(j);
                    }

                    j++;
                }
                i++;
            }

            return LooseGraphs;
        }

        return LooseGraphs;
    }
    
    public bool linkNodes(Node<T> link, Node<T> link2)
    {
        if (link == null || link2 == null || !containsNode(link) || !containsNode(link2)) 
        {
            return false;
        }

        link2.links.Add(link2.links.Count, link);
        link.links.Add(link.links.Count, link2);

        return true;
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