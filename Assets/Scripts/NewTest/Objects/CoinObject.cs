using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObject : PickupableObject
{
    public int amount = 0;
    public PickUpObject po = PickUpObject.Coin;
    public bool instantPickup;

    // Use this for initialization
    void Start()
    {
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
        player.collectedCoins += amount;
        player.pss.PlayPickup(po);
        if (player.coinParticleSystem != null) player.coinParticleSystem.Play();
    }
}
