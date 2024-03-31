using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TS;
using Unity.VisualScripting;
using UnityEngine;

public class PriorityQueueFloat
{
    private SortedDictionary<float, Queue<NodeInfo>> queue = new();

    public SortedDictionary<float, Queue<NodeInfo>> Queue => queue;

    public int Count => queue.Count;

    public void Enqueue(float _priority, NodeInfo _item)
    {
        if (!queue.ContainsKey(_priority))
        {
            queue[_priority] = new Queue<NodeInfo>();
        }

        queue[_priority].Enqueue(_item);
        _item.priority = _priority;
    }

    public NodeInfo Dequeue()
    {
        if (queue.Count == 0)
        {
            return default(NodeInfo);
        }

        Queue<NodeInfo> highestPriorityQueue = queue.First().Value;

        NodeInfo item = highestPriorityQueue.Dequeue();

        if (highestPriorityQueue.Count == 0)
            queue.Remove(queue.First().Key);

        return item;
    }

    public float FindKeyOfItem(NodeInfo _item)
    {
        foreach (KeyValuePair<float, Queue<NodeInfo>> keyValuePair in queue)
        {
            if (keyValuePair.Value.Contains(_item))
                return keyValuePair.Key;
        }

        return -1;
    }

    public void RemoveItem(float _key, NodeInfo _item)
    {
        if (!queue[_key].Contains(_item))
            return;

        Queue<NodeInfo> tempQueue = new();

        while (queue[_key].Count > 0)
        {
            NodeInfo item = queue[_key].Dequeue();

            if (!ReferenceEquals(item, _item))
            {
                tempQueue.Enqueue(item);
            }
        }

        while (tempQueue.Count > 0)
        {
            queue[_key].Enqueue(tempQueue.Dequeue());
        }

        if (queue[_key].Count == 0)
        {
            queue.Remove(_key);
        }
    }

    public void ChangePriority(float _oldPriority, NodeInfo _item, float _newPriority)
    {
        RemoveItem(_oldPriority, _item);

        Enqueue(_newPriority, _item);
    }

    public void DisplayPriorityQueue()
    {
        Debug.Log("***********************");

        int index = 1;
        foreach (KeyValuePair<float, Queue<NodeInfo>> kvp in queue)
        {
            Debug.Log($"Queue {index++} Priority [{kvp.Key}] count : " + kvp.Value.Count);
        }

        Debug.Log("***********************");
    }
}
