using UnityEngine;
using System.Collections;

interface ITCollection<T>
{
    /// <summary>
    /// Retrieves the object with the lowest value in the collection.
    /// </summary>
    /// <returns>The object with the lowest value</returns>
    T Min();

    /// <summary>
    /// Returns the object with the highest value in the collection.
    /// </summary>
    /// <returns>The object with the highest value</returns>
    T Max();

    /// <summary>
    /// Returns and removes the object with the lowest value from the collection.
    /// </summary>
    /// <returns>The object with the lowest value</returns>
    T DequeueMin();

    /// <summary>
    /// Returns and removes the object with the highest value from the collection.
    /// </summary>
    /// <returns></returns>
    T DequeueMax();

    /// <summary>
    /// Adds the given object to the collection.
    /// </summary>
    /// <param name="item">The item you want to add to the collection</param>
    void Enqueue(T item);

    /// <summary>
    /// Removes the given object from the collection.
    /// </summary>
    /// <param name="item">The item that needs to be removed from the collection</param>
    /// <returns>Wether the object has been deleted or not</returns>
    bool Remove(T item);

    /// <summary>
    /// The size of the collection.
    /// </summary>
    int Count { get; }
}
