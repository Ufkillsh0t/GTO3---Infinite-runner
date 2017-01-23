using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableObject : SpawnableObject {

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

    // Use this for initialization
    protected void Awake () {
        base.Awake();
        curPickupSpeed = startPickupSpeed;
        curRotateSpeed = startRotateSpeed;
    }

    protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected void Update () {
        base.Update();
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
    public virtual void MoveTowardsPlayer()
    {
        if (rotateOnSmoothPickup)
        {
            if (curRotateSpeed < maxRotateSpeed)
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
    /// Resets the position of this object.
    /// </summary>
    /// <param name="smoothPickup"></param>
    /// <param name="visible"></param>
    public void ResetPosition(bool smoothPickup = false, bool visible = false)
    {
        base.ResetPosition(false);
        this.smoothPickup = smoothPickup;
    } 

    /// <summary>
    /// Pickups the object.
    /// </summary>
    public virtual void PickUp()
    {
        ResetPosition(false);
        used = false;
        Hide();
    }

    /// <summary>
    /// Sets the smooth pickup boolean to true so this object will be pickedup smoothly near the player.
    /// </summary>
    public virtual void SmoothPickup()
    {
        smoothPickup = true;
    }
}
