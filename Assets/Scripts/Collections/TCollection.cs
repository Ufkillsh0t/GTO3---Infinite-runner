﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


internal class TCollection<T> : ITCollection<T> where T : IValue
{
    private T[] items;
    private int size;
    private int itemCount;

    public TCollection()
        : this(16)
    {

    }

    public TCollection(int size)
    {
        this.size = size;
        items = new T[size];
    }

    public int Count
    {
        get
        {
            return itemCount;
        }
    }

    public T DequeueMax()
    {
        int maxIndex = MaxIndex();
        T max = items[maxIndex];
        items = ExtensionMethods.RemoveAt(items, maxIndex);
        itemCount--;
        return max;
    }

    public T DequeueMin()
    {
        int minIndex = MinIndex();
        T min = items[minIndex];
        items = ExtensionMethods.RemoveAt(items, minIndex);
        itemCount--;
        return min;
    }

    public void Enqueue(T item)
    {
        if (itemCount >= size)
        {
            var temp = items;
            size *= 2;
            items = new T[size];
            Array.Copy(temp, items, temp.Length);
        }
        items[itemCount] = item;
        itemCount++;
    }

    public T Max()
    {
        return items[MaxIndex()];
    }

    private int MaxIndex()
    {
        int highestIndex = 0;
        for (int i = 1; i < itemCount; i++)
        {
            if (items[highestIndex].GetValue() < items[i].GetValue()) highestIndex = i;
        }
        return highestIndex;
    }

    public T Min()
    {
        return items[MinIndex()];
    }

    private int MinIndex()
    {
        int lowestIndex = 0;
        for (int i = 1; i < itemCount; i++)
        {
            if (items[lowestIndex].GetValue() > items[i].GetValue()) lowestIndex = i;
        }
        return lowestIndex;
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < itemCount; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item))
            {
                items = ExtensionMethods.RemoveAt(items, i);
                return true;
                //items = items.Where((source, index) => index != i).ToArray();
            }
        }
        return false;
    }
}

public static class ExtensionMethods
{
    /// <summary>
    /// First i wanted to use linq to get a new array but that is way to slow, this is the fastest solution.
    /// http://stackoverflow.com/questions/457453/remove-element-of-a-regular-array
    /// </summary>
    /// <typeparam name="T">The type of object</typeparam>
    /// <param name="source">The array</param>
    /// <param name="index">Index of the item.</param>
    /// <returns>The renewed array</returns>
    internal static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] dest = new T[source.Length];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index); //Kopieert het gedeelte van de items array tot het item waarvan je de index gaf in dest.

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1); //Kopieert het gedeelte van de items array in dest na het item waarvan je de index gaf.

        return dest;
    }
}
