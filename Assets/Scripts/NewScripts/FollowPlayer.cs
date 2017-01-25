using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    //public GameObject ObjectToFollowPlayer;
    public float dampTime = 0.15f;
    private PlayerScript ps;
    private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
        ps = PlayerScript.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        SmoothlyFollow();
	}

    public void SmoothlyFollow()
    {
        Vector3 target = new Vector3(ps.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, dampTime);
    }
}
