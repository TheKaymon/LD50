using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeapItem<T> : IComparable<T>
{ 
    int HeapIndex
    {
        get;
        set;
    }
}

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemCount;
    public int Count => (currentItemCount);

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public void UpdateItem( T item )
    {
        SortUp(item);
        // SortDown();
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public bool Contains( T item )
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown( T item )
    {
        while( true )
        {
            int leftChildIndex = item.HeapIndex * 2 + 1;
            int rightChildIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if ( leftChildIndex < currentItemCount )
            {
                swapIndex = leftChildIndex;

                if ( rightChildIndex < currentItemCount )
                {
                    if ( items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0 )
                    {
                        swapIndex = rightChildIndex;
                    }
                }

                if ( item.CompareTo(items[swapIndex]) < 0 )
                    Swap(item, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }
    
    private void SortUp( T item)
    {
        int parentIndex = (item.HeapIndex-1)/ 2;

        while( true )
        {
            T parentItem = items[parentIndex];
            if ( item.CompareTo(parentItem) > 0 )
            {
                Swap(item, parentItem);
            }
            else
                break;

            parentIndex = ( item.HeapIndex - 1 ) / 2;
        }
    }

    public void Swap( T a, T b )
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;

        int tempIndex = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = tempIndex;
    }
}
