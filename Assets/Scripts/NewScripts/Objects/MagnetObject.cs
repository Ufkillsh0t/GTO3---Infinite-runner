using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetObject : ItemObject {

    public PickUpObject po = PickUpObject.Magnet;
    public float duration = 12f;

    // Use this for initialization
    protected void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected void Update () {
        base.Update();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        base.PickUp();
        if (!player.magnetUsed) player.magnetUsed = true;
        if (player.magnetParticleSystem != null) player.magnetParticleSystem.Play();
        player.MagnetDuration = duration;
        player.pss.PlayPickup(po);
    }
}
