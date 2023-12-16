using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// This is an implementation of a doubly linked list, but safe.
/// It requires that there is always at least one node in the list to prevent a NullPointerException.
/// Used in my implementation of enemy AI.
/// </summary>
public class SafelyLinkedList<T>
{
    public Node Head {get; private set;}
    /// <summary>The last node in the list. This will never change and will never be null </summary>
    public Node Tail {get; private set;}
    public int size {get; private set;}

    public SafelyLinkedList(T data)
    {
        if (data == null)
        {
            throw new Exception("The data in the tail node of a SafelyLinkedList cannot be null");
        }
        Head = new Node(data);
        Tail = Head;
        size = 1;
    }

    public void Add(T data)
    {
        Head = new Node(data, Head);
        Head.nextNode.previousNode = Head;
        size++;
    }

    /// <summary>
    /// Removes an item with the specified data from the list.
    /// </summary>
    /// <returns>
    /// Returns the data of the node that was removed
    /// </returns>
    /// <exception cref="Throws an exception if the node doesn't exist, or is the tail node">
    /// </exception>
    public T Remove(T data)
    {
        Node currentNode = Head;
        while (currentNode != null)
        {
            if (currentNode == Tail && Tail.data.Equals(data))
            {
                throw new Exception("Cannot remove tail node from a SafelyLinkedList");
            }
            if (currentNode.data.Equals(data))
            {
                currentNode.nextNode.previousNode = currentNode.previousNode;
                if (currentNode.previousNode != null)
                {
                    currentNode.previousNode.nextNode = currentNode.nextNode;
                }
                size--;
                return currentNode.data;
            }
            currentNode = currentNode.nextNode;
        }
        throw new Exception("Failed to find the specified item");
    }

    public T Remove(int index)
    {
        if (index < 0 || index >= size) {
            throw new IndexOutOfRangeException();
        }

        Node currentNode = Head;
        for (int i = 0; i < index; i++) {currentNode = currentNode.nextNode;}

        if (currentNode == Tail)
        {
            throw new Exception("Cannot remove tail node from a SafelyLinkedList");
        }
        currentNode.nextNode.previousNode = currentNode.previousNode;
        currentNode.previousNode.nextNode = currentNode.nextNode;
        size--;
        return currentNode.data;
    }

    public void Add(int index, T data)
    {
        if (index < 0 || index >= size) {
            throw new IndexOutOfRangeException();
        }

        Node currentNode = Head;
        for (int i = 0; i < index; i++) {currentNode = currentNode.nextNode;}

        Node newNode = new Node(data, currentNode.nextNode, currentNode);
        currentNode.nextNode = newNode;
        if (currentNode == Tail) {Tail = newNode;}

        size++;
    }

    public void Remove()
    {
        Head = Head.nextNode;
        size--;
    }

    public void Insert(T data, Comparer<T> comparer)
    {
        Node newNode = new Node(data);
        Node currentNode = Head;
        while (currentNode.nextNode != null)
        {
            switch (comparer.Compare(data, currentNode.data))
            {
                case -1:
                    break;
                case 0:
                    newNode.nextNode = currentNode.nextNode;
                    newNode.previousNode = currentNode.previousNode;
                    currentNode.nextNode.previousNode = newNode;
                    currentNode.nextNode = newNode;
                    return;
                case 1:
                    newNode.nextNode = currentNode;
                    newNode.previousNode = currentNode.previousNode;
                    currentNode.nextNode.previousNode = newNode;
                    currentNode.nextNode = newNode;
                    return;
                default:
                    break;
            }
        }
    }

    private Node getNode(T data)
    {
        Node currentNode = Head;
        while (currentNode != null)
        {
            if (currentNode.data.Equals(data)) {return currentNode;}
            currentNode = currentNode.nextNode;
        }
        throw new Exception("Failed to find the specified item");
    }

    public void Swap(T firstItem, T secondItem)
    {
        Node firstNode = getNode(firstItem);
        Node secondNode = getNode(secondItem);

        Node clonedFirstNode = firstNode.Clone();

        firstNode.nextNode = secondNode.nextNode;
        firstNode.previousNode = secondNode.previousNode;
        secondNode.nextNode = clonedFirstNode.nextNode;
        secondNode.previousNode = clonedFirstNode.previousNode;
    }

    public void Swap(Node firstNode, Node secondNode)
    {
        Node clonedFirstNode = firstNode.Clone();

        firstNode.nextNode = secondNode.nextNode;
        firstNode.previousNode = secondNode.previousNode;
        secondNode.nextNode = clonedFirstNode.nextNode;
        secondNode.previousNode = clonedFirstNode.previousNode;
    }

    public void Swap(Node node, T secondNodeData)
    {
        Node secondNode = getNode(secondNodeData);
        Node clonedFirstNode = node.Clone();

        node.nextNode = secondNode.nextNode;
        node.previousNode = secondNode.previousNode;
        secondNode.nextNode = clonedFirstNode.nextNode;
        secondNode.previousNode = clonedFirstNode.previousNode;
    }

    public class Node
    {
        public Node previousNode;
        public Node nextNode;
        public T data;

        public Node(T data, Node nextNode, Node previousNode)
        {
            this.previousNode = previousNode;
            this.nextNode = nextNode;
            this.data = data;
        }

        public Node(T data, Node nextNode)
        {
            this.nextNode = nextNode;
            this.data = data;
        }
        
        public Node(T data)
        {
            this.data = data;
        }

        public Node Clone()
        {
            return new Node(data, nextNode, previousNode);
        }
    }
}
