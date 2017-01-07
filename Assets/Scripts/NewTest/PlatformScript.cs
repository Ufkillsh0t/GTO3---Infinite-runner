using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

    public SpawnableObject[] platformObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveResize(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Vector3 postion)
    {
        Resize(minX, maxX, minY, maxY, minZ, maxZ);
        transform.position = postion;
    }

    public void Resize(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        Resize(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
    }

    public void Resize(float x, float y, float z)
    {
        Vector3 scale = new Vector3(x, y, z);
        Resize(scale);
    }
    
    public void Resize(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
