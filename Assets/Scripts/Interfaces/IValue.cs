using UnityEngine;
using System.Collections;

interface IValue
{
    /// <summary>
    /// Gets the float value of the object.
    /// </summary>
    /// <returns>The value of this object</returns>
    float Value { get; }

    /// <summary>
    /// Gets wether the object has been used or not.
    /// If the object is used it won't be removed from the collection.
    /// </summary>
    /// <returns>Wether the object is ijn use or not.</returns>
    bool IsObjectUsed { get; }
}
