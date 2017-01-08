﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnedObjectType
{
    None,
    Coin,
    Item,
    Obstacle
}

public enum SpawnObject
{
    None,
    SmallCoin,
    MediumCoin,
    BigCoin,
    Magnet,
    SpeedBoost,
    SlowDown,
    PlatformIncrease,
    PlatformDecrease,
    Spike,
    Box,
    Mud,
    HoveringBox
}

public class SpawnableObject : MonoBehaviour, IComparable<SpawnableObject>
{
    public Vector3 resetPosition = Vector3.zero;
    public SpawnedObjectType spawnedObjectType = SpawnedObjectType.None;
    public SpawnObject spawnType = SpawnObject.None;
    public bool rotateOnSmoothPickup = true;
    public bool lookAtPlayerIfNoRotateSmoothPickup = true;
    public bool pickUpObjectSmoothly = true;
    public float pickupRange = 0.3f;
    public float startPickupSpeed = 3f;
    public float pickupSpeedIncreasePerSecond = 17f;
    public float maxPickupSpeed = 20f;
    public float startRotateSpeed = 2f;
    public float maxRotateSpeed = 30f;
    public float rotateSpeedIncreasePerSecond = 20f;

    private bool smoothPickup = true;
    private float curPickupSpeed;
    private float curRotateSpeed;

    private PlayerScript player;

    protected void Awake()
    {
        player = PlayerScript.Instance;
        curPickupSpeed = startPickupSpeed;
        curRotateSpeed = startRotateSpeed;
    }

    protected void Update()
    {
        if (smoothPickup)
        {
            if (pickUpObjectSmoothly)
            {
                MoveTowardsPlayer();
            }
            else
            {
                PickUp();
            }
        }
    }

    /// <summary>
    /// Moves this object towards the player.
    /// </summary>
    public void MoveTowardsPlayer()
    {
        if (rotateOnSmoothPickup)
        {
            if(curRotateSpeed < maxRotateSpeed)
            {
                float temp = curRotateSpeed + (rotateSpeedIncreasePerSecond * Time.deltaTime);
                curRotateSpeed = (temp >= maxRotateSpeed) ? maxRotateSpeed : temp;
            }
            transform.Rotate(curRotateSpeed, 0, 0); //Nog de goede rotatie vinden.
        }
        else if (lookAtPlayerIfNoRotateSmoothPickup)
        {
            transform.LookAt(player.transform);
        }

        if (curPickupSpeed < maxPickupSpeed)
        {
            float temp = curPickupSpeed + (pickupSpeedIncreasePerSecond * Time.deltaTime);
            curPickupSpeed = (temp >= maxPickupSpeed) ? maxPickupSpeed : temp;
        }
        float step = curPickupSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        if (Vector3.Distance(transform.position, player.transform.position) < pickupRange)
        {
            PickUp();
        }
    }

    /// <summary>
    /// Pickups the object.
    /// </summary>
    public void PickUp()
    {
        ResetPosition(false, false);
    }

    /// <summary>
    /// Sets the smooth pickup boolean to true so this object will be pickedup smoothly near the player.
    /// </summary>
    public void SmoothPickup()
    {
        smoothPickup = true;
    }

    /// <summary>
    /// Resets the objects position.
    /// </summary>
    /// <param name="visible">If the object needs to be visible or not.</param>
    public void ResetPosition(bool visible, bool smoothPickUp)
    {
        smoothPickUp = false;
        Spawn(resetPosition, visible);
    }

    /// <summary>
    /// Spawns this objecton a new position
    /// </summary>
    /// <param name="position">The position you want to spawn the object on.</param>
    /// <param name="visible">If the object needs to be visible on that position or not.</param>
    public void Spawn(Vector3 position, bool visible)
    {
        transform.position = position;
        gameObject.SetActive(visible);
    }

    /// <summary>
    /// Makes the object visible again.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the object
    /// </summary>
	public void Hide()
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