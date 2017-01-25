using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObject : PickupableObject
{
    public int amount = 0;
    public PickUpObject po = PickUpObject.Coin;
    public bool instantPickup;
    private Quaternion startRotation;

    // Use this for initialization
    void Start()
    {
        startRotation = transform.rotation;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CoinPickUp();
        }
    }

    public override void PickUp()
    {
        if (instantPickup)
        {
            CoinPickUp();
        }
        else
        {
            SmoothPickup();
        }
    }

    private void CoinPickUp()
    {
        base.PickUp();
        transform.rotation = startRotation;
        player.collectedCoins += amount;
        player.pss.PlayPickup(po);
        if (player.coinParticleSystem != null) player.coinParticleSystem.Play();
    }
}
