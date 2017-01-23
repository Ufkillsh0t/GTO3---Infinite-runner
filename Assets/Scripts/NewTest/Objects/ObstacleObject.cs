using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : SpawnableObject {

	// Use this for initialization
	protected void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected void Update () {
        base.Update();
    }

    public void SetPlayerSpeed(float speed)
    {
        player.SetSpeed(speed);
    }

    public void KillPlayer()
    {
        player.Death();
    }
}
