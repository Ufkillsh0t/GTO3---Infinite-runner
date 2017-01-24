using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObject : ItemObject
{

    public NextFloor nextFloor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        player.GetPlatForm.GetNextPlatform(player.transform.position, nextFloor).MovePlayer(player);
    }
}
