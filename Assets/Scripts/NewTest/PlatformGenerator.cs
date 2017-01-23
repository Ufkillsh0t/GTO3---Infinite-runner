using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

    private Vector3 platformNextPosition;
    public int numberOfPlatforms = 20;
    private Queue<Transform> platformQueue;
    public Transform platformPrefab;
    public float platformRecycleOffset;
    public Vector3 platformStartPosition;
    public Vector3 platformMinSize, platformMaxSize, platformMinGap, platformMaxGap;
    public float platformMinY, platformMaxY;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
