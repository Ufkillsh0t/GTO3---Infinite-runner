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

    public float Value { get { return GetRange(); } }

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
        PickUp();
    }

    public void PickUp()
    {
        transform.position = startPosition;
        player.collectedCoins++;
        //pickedUp = true; PickedUp kan er pas in wanneer er een animatie van de coin naar de player in zit.
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
}
