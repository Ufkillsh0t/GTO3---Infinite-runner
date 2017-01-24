using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : PlatformScript
{

    public Vector3 maxMovement = Vector3.right;
    public Vector3 minMovement = Vector3.left;
    public Vector3 maxPos;
    public Vector3 minPos;
    public float movementSpeed = 4f;
    public bool switchedDirection = false;

    // Use this for initialization
    void Awake()
    {
        maxPos = new Vector3(transform.position.x + maxMovement.x,
            transform.position.y + maxMovement.y,
            transform.position.z + maxMovement.z);
        minPos = new Vector3(transform.position.x + minMovement.x,
            transform.position.y + minMovement.y,
            transform.position.z + minMovement.z);
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        switch (movementAxis)
        {
            case MovementAxis.X:
                MoveX();
                break;
            case MovementAxis.Y:
                MoveY();
                break;
            case MovementAxis.Z:
                MoveZ();
                break;
            default:
                Debug.LogError("MovementType not set to an axis!");
                break;
        }
    }

    public void MoveX()
    {
        float step = movementSpeed * Time.deltaTime;
        if (switchedDirection)
        {
            transform.localPosition = new Vector3(transform.localPosition.x - step,
                transform.localPosition.y,
                transform.localPosition.z);
            if (transform.localPosition.x < minPos.x)
            {
                transform.localPosition = new Vector3(
                    minPos.x,
                    transform.localPosition.y,
                    transform.localPosition.z);
                switchedDirection = false;
            }
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x + step,
                transform.localPosition.y,
                transform.localPosition.z);
            if (transform.localPosition.x > maxPos.x)
            {
                transform.localPosition = new Vector3(
                    maxPos.x,
                    transform.localPosition.y,
                    transform.localPosition.z);
                switchedDirection = true;
            }
        }
    }

    public void MoveY()
    {
        float step = movementSpeed * Time.deltaTime;
        if (switchedDirection)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y - step,
                transform.localPosition.z);
            if (transform.localPosition.y < minPos.y)
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    minPos.y,
                    transform.localPosition.z);
                switchedDirection = false;
            }
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y + step,
                transform.localPosition.z);
            if (transform.localPosition.y > maxPos.y)
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    maxPos.y,
                    transform.localPosition.z);
                switchedDirection = true;
            }
        }
    }

    public void MoveZ()
    {
        float step = movementSpeed * Time.deltaTime;
        if (switchedDirection)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z - step);
            if (transform.localPosition.z < minPos.z)
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y,
                    minPos.z);
                switchedDirection = false;
            }
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z + step);
            if (transform.localPosition.z > maxPos.z)
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y,
                    maxPos.z);
                switchedDirection = true;
            }
        }
    }

    public override float GetMaxMovement(MovementAxis ma)
    {
        switch (ma)
        {
            case MovementAxis.X:
                return maxMovement.x;
            case MovementAxis.Y:
                return maxMovement.y;
            case MovementAxis.Z:
                return maxMovement.z;
            default:
                return 0;
        }
    }

    public override float GetMinMovement(MovementAxis ma)
    {
        switch (ma)
        {
            case MovementAxis.X:
                return minMovement.x;
            case MovementAxis.Y:
                return minMovement.y;
            case MovementAxis.Z:
                return minMovement.z;
            default:
                return 0;
        }
    }

    public override float GetMovingDistance(MovementAxis ma)
    {
        switch (ma)
        {
            case MovementAxis.X:
                return maxMovement.x - minMovement.x;
            case MovementAxis.Y:
                return maxMovement.y - minMovement.y;
            case MovementAxis.Z:
                return maxMovement.z - minMovement.z;
            default:
                return 0;
        }
    }
}
