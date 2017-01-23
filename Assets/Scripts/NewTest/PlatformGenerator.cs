using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour, IPlatform
{
    [Serializable]
    public class Floor
    {
        [NonSerialized]
        public Vector3 platformNextPosition;
        public int numberOfPlatforms;
        //public Vector3 platformStartScale;
        public Vector3 platformMinSize, platformMaxSize, platformMinGap, platformMaxGap;
        public float platformMinY, platformMaxY;
        public Vector3 platformStartPosition;
        [NonSerialized]
        public Queue<PlatformScript> platformQueue;
    }

    //public Vector3 startPosition;
    public Vector3 minPlatformDifference, maxPlatformDifference;
    private Vector3 prevLayerPos;
    public Transform platformPrefab;
    public float platformRecycleOffset;
    public int numberOfFloors = 3;
    public Floor[] platformQueues;
    private PlayerScript player;
    private bool first = true;

    // Use this for initialization
    void Start()
    {
        player = PlayerScript.Instance;
        InstantiatePlatforms();
    }

    private void InstantiatePlatforms()
    {
        if (platformPrefab != null && platformPrefab.GetComponent<PlatformScript>() != null)
        {
            if (platformQueues.Length > 0)
            {
                for (int floor = 0; floor < numberOfFloors; floor++)
                {
                    Queue<PlatformScript> platformQueue = new Queue<PlatformScript>(platformQueues[floor].numberOfPlatforms);
                    for (int i = 0; i < platformQueues[floor].numberOfPlatforms; i++)
                    {
                        platformQueue.Enqueue(Instantiate(platformPrefab).GetComponent<PlatformScript>());
                    }
                    platformQueues[floor].platformQueue = platformQueue;
                    platformQueues[floor].platformNextPosition = platformQueues[floor].platformStartPosition;
                    for (int i = 0; i < platformQueues[floor].numberOfPlatforms; i++)
                    {
                        Recycle(platformQueues[floor]);
                    }
                }
            }
            else
            {
                Debug.LogError("Please make atleast one default floor to spawn platforms on!");
            }
        }
        else
        {
            Debug.LogError("Could not generate platforms due to missing prefab or the platform hasn't got the component PlatformScript!");
        }
    }

    // Update is called once per frame
    void Update()
    {    
        for (int i = 0; i < platformQueues.Length; i++)
        {
            if (platformQueues[i].platformQueue.Peek().transform.localPosition.x + platformRecycleOffset < player.transform.position.x)
            {
                Recycle(platformQueues[i]);
            }
        }
    }

    private void Recycle(Floor floor)
    {
        Vector3 scale;
        if (floor.platformStartPosition == floor.platformNextPosition)
        {
            scale = new Vector3(
                12,
                UnityEngine.Random.Range(floor.platformMinSize.y, floor.platformMaxSize.y),
                UnityEngine.Random.Range(floor.platformMinSize.z, floor.platformMaxSize.z)
                );
        }
        else
        {
            scale = new Vector3(
                UnityEngine.Random.Range(floor.platformMinSize.x, floor.platformMaxSize.x),
                UnityEngine.Random.Range(floor.platformMinSize.y, floor.platformMaxSize.y),
                UnityEngine.Random.Range(floor.platformMinSize.z, floor.platformMaxSize.z)
                );
        }

        Vector3 position = floor.platformNextPosition;
        position.x += scale.x * 0.5f;
        position.y += scale.y * 0.5f;
        //position.z += scale.z * 0.5f;

        PlatformScript platformScript = floor.platformQueue.Dequeue();
        platformScript.transform.localScale = scale;
        platformScript.transform.localPosition = position;
        //platformScript.transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        floor.platformQueue.Enqueue(platformScript);

        if(first && player != null)
        {
            first = false;
            player.transform.position = new Vector3(position.x, position.y + scale.y, position.z);
        }

        if (floor.platformNextPosition != floor.platformStartPosition)
        {
            //latestMagnet--;
            //SpawnGridsPlatform(position, scale);
        }

        floor.platformNextPosition += new Vector3(
            UnityEngine.Random.Range(floor.platformMinGap.x, floor.platformMaxGap.x) + scale.x,
            UnityEngine.Random.Range(floor.platformMinGap.y, floor.platformMaxGap.y),
            UnityEngine.Random.Range(floor.platformMinGap.z, floor.platformMaxGap.z));

        if (floor.platformNextPosition.y < floor.platformMinY)
        {
            floor.platformNextPosition.y = floor.platformMinY + floor.platformMaxGap.y;
        }
        else if (floor.platformNextPosition.y > floor.platformMaxY)
        {
            floor.platformNextPosition.y = floor.platformMaxY - floor.platformMaxGap.y;
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        //CoinScript coin = coinQueue.DequeueMax();
        //Transform coinTransform = coin.transform;
        //Vector3 spawnPos = new Vector3(position.x + coinTransform.localScale.x, position.y + coinTransform.localScale.y, position.z + coinTransform.localScale.z);
        //coinTransform.position = spawnPos;
        //coin.pickedUp = false;
        //coin.enabled = true;
        //coinQueue.Enqueue(coin);
    }

    public void PickupCoin(float range)
    {
        //CoinScript coin = coinQueue.DequeueMin(range);
        //if (coin != null)
        {
            //coin.PickUp();
            //coinQueue.Enqueue(coin);
        }
    }

    private void SpawnMagnet(Vector3 position)
    {
        //Transform magnet = magnetQueue.Dequeue();
        //Vector3 spawnPos = new Vector3(position.x + magnet.localScale.x, position.y, position.z + magnet.localScale.z);
        //magnet.position = spawnPos;
        //magnetQueue.Enqueue(magnet);
    }

    public void Pickup(float range)
    {
        //throw new NotImplementedException();
    }
}
