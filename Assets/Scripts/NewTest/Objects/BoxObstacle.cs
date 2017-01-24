using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObstacle : ObstacleObject {

    public float playerSpeed = 0.6f;

	// Use this for initialization
	void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            SlowPlayer();
        }
    }

    public void SlowPlayer()
    {
        SetPlayerSpeed(playerSpeed);
        ResetPosition(false);
        Hide();
    }
}
