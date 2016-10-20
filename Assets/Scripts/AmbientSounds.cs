using UnityEngine;
using System.Collections;

public class AmbientSounds : MonoBehaviour {

    private static PlayerScript player;

    // Use this for initialization
    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
