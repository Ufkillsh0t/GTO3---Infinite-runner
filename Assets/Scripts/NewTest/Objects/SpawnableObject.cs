using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour, IComparable<SpawnableObject>, IValue
{
    public Vector3 resetPosition = Vector3.zero;
    public SpawnedObjectType spawnedObjectType = SpawnedObjectType.None;
    public bool used = false;

    protected PlayerScript player = PlayerScript.Instance;

    public float Value
    {
        get
        {
            return GetRange();
        }
    }

    public bool IsObjectUsed
    {
        get
        {
            return used;
        }
    }

    protected virtual void Awake()
    {
        //player = PlayerScript.Instance;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Resets the objects position.
    /// </summary>
    /// <param name="visible">If the object needs to be visible or not.</param>
    public virtual void ResetPosition(bool visible = true)
    {
        Spawn(resetPosition, visible);
    }

    /// <summary>
    /// Spawns this objecton a new position
    /// </summary>
    /// <param name="position">The position you want to spawn the object on.</param>
    /// <param name="visible">If the object needs to be visible on that position or not.</param>
    public virtual void Spawn(Vector3 position, bool visible = true)
    {
        transform.position = position;
        gameObject.SetActive(visible);
    }

    /// <summary>
    /// Makes the object visible again.
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the object
    /// </summary>
	public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Compares the range difference between the spawnable objects and return wether this object is lower than the other.
    /// </summary>
    /// <param name="other">The object you want to compare this object to</param>
    /// <returns>A integer which indexes this object based on Range</returns>
    public int CompareTo(SpawnableObject other)
    {
        if (GetRange() < other.GetRange())
        {
            return 1;
        }
        else if (GetRange() == other.GetRange())
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Checks wether the player is within the pickUpRange
    /// </summary>
    /// <returns>If the player is within the pickUpRange</returns>
    public bool withinRange()
    {
        return (GetRange() < player.magnetPickupRange);
    }


    /// <summary>
    /// Gets the range between the player and the spawnable object.
    /// </summary>
    /// <returns>The range between the player and the spawable object.</returns>
    private float GetRange()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }
}
