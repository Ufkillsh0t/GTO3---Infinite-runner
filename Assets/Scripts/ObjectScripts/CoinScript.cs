using UnityEngine;
using System.Collections;
using System;

public class CoinScript : MonoBehaviour, IComparable<CoinScript>, IValue
{

    private static PlayerScript player;
    private Vector3 startPosition;
    public int coinValue = 1;
    public bool pickedUp = false;
    private PickUpObject po = PickUpObject.Coin;

    public float GetRangeTestProperty { get { return GetRange(); } }

    public bool IsObjectUsed { get { return pickedUp; } }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        startPosition = transform.position;
    }

    // Use this for initialization
    void Start()
    {
        if (player == null) Debug.LogWarning("Player script could not be found!");
    }

    void OnTriggerEnter(Collider other)
    {
        transform.position = startPosition;
        player.collectedCoins++;
        pickedUp = true;
        //this.enabled = false;
        player.pss.PlayPickup(po);
        //Debug.Log("I got triggered!");
    }

    public int CompareTo(CoinScript other)
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

    public bool withinRange()
    {
        return (GetRange() < player.magnetPickupRange);
    }

    private float GetRange()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public float GetValue()
    {
        return GetRange();
    }
}
