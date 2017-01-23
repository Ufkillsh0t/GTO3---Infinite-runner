using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour, IPlatform
{
    public const float minSpawnObjectChance = 0f;
    public const float maxSpawnObjectChance = 100f;

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

    [Serializable]
    public class CoinSpawn
    {
        private SpawnedObjectType spawnedObjectType = SpawnedObjectType.Coin;
        public GameObject coinObject;
        [Range(minSpawnObjectChance, maxSpawnObjectChance)]
        public float spawnObjectChance;
    }

    //public Vector3 startPosition;
    public Vector3 minPlatformDifference, maxPlatformDifference;
    private Vector3 prevLayerPos;
    public Transform platformPrefab;
    public float platformRecycleOffset;
    public int numberOfFloors = 3;
    public Floor[] platformQueues;
    public int amountOfCoins = 70;
    public CoinSpawn[] coinSpawns;
    private PlayerScript player;
    private bool first = true;

    private TCollection<CoinObject> tCoin;
    private TCollection<ItemObject> tItem;
    private TCollection<ObstacleObject> tObstacle;

    // Use this for initialization
    void Start()
    {
        player = PlayerScript.Instance;
        InstantiateCoins();
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

    private void InstantiateCoins()
    {
        if (coinSpawns.Length > 0)
        {
            tCoin = new TCollection<CoinObject>(amountOfCoins);
            for (int i = 0; i < amountOfCoins; i++)
            {
                GameObject co = InstantiateRandomCoinObject();
                if (co != null)
                {
                    tCoin.Enqueue(Instantiate(co).GetComponent<CoinObject>());
                }
                else
                {
                    Debug.LogError("Coinobject is null!");
                }
            }
        }
        else
        {
            Debug.LogError("Atleast one coin object is needed!");
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
        PlatformScript platformScript = floor.platformQueue.Dequeue();
        if (floor.platformStartPosition == floor.platformNextPosition)
        {
            platformScript.MoveResize(12, 12,
                floor.platformMinSize.y, floor.platformMaxSize.y,
                floor.platformMinSize.z, floor.platformMaxSize.z,
                floor.platformStartPosition);
        }
        else
        {
            platformScript.MoveResize(floor.platformMinSize.x, floor.platformMaxSize.x,
            floor.platformMinSize.y, floor.platformMaxSize.y,
            floor.platformMinSize.z, floor.platformMaxSize.z,
            floor.platformNextPosition);
            SpawnObjects(platformScript.GenerateSpawnObjectTypesArray(), platformScript);
        }
        platformScript.renderer.material.color = Color.HSVToRGB(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
        Vector3 position = floor.platformNextPosition;
        position.x += platformScript.transform.localScale.x * 0.5f;
        position.y += platformScript.transform.localScale.y * 0.5f;
        //position.z += scale.z * 0.5f;


        if (first && player != null)
        {
            first = false;
            player.transform.position = new Vector3(platformScript.transform.position.x, 
                platformScript.transform.position.y + platformScript.transform.localScale.y,
                platformScript.transform.position.z);
        }

        if (floor.platformNextPosition != floor.platformStartPosition)
        {
            //latestMagnet--;
            //SpawnGridsPlatform(position, scale);
        }

        floor.platformNextPosition += new Vector3(
            UnityEngine.Random.Range(floor.platformMinGap.x, floor.platformMaxGap.x) + platformScript.transform.localScale.x,
            UnityEngine.Random.Range(floor.platformMinGap.y, floor.platformMaxGap.y),
            UnityEngine.Random.Range(floor.platformMinGap.z, floor.platformMaxGap.z));
        floor.platformQueue.Enqueue(platformScript);

        if (floor.platformNextPosition.y < floor.platformMinY)
        {
            floor.platformNextPosition.y = floor.platformMinY + floor.platformMaxGap.y;
        }
        else if (floor.platformNextPosition.y > floor.platformMaxY)
        {
            floor.platformNextPosition.y = floor.platformMaxY - floor.platformMaxGap.y;
        }
    }

    private void SpawnObjects(SpawnedObjectType[,] spawns, PlatformScript ps)
    {
        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            for (int y = 0; y < spawns.GetLength(1); y++)
            {
                switch (spawns[x, y])
                {
                    case SpawnedObjectType.None:
                        break;
                    case SpawnedObjectType.Coin:
                        SpawnCoin(ps, x, y);
                        break;
                    case SpawnedObjectType.Item:
                        break;
                    case SpawnedObjectType.Obstacle:
                        break;
                }
            }
        }
    }

    private GameObject InstantiateRandomCoinObject()
    {
        float spawnFloat = UnityEngine.Random.Range(minSpawnObjectChance, maxSpawnObjectChance);
        foreach (CoinSpawn cs in coinSpawns)
        {
            if (spawnFloat <= cs.spawnObjectChance)
            {
                return cs.coinObject;
            }
        }
        return null;
    }

    private void SpawnCoin(PlatformScript ps, int x, int y)
    {
        //Debug.Log("Spawn coin:" + x + "-" + y);
        CoinObject coin = tCoin.DequeueMax();
        Transform coinTransform = coin.transform;
        ps.SpawnObject(coin, x, y);
        //Vector3 spawnPos = new Vector3(position.x + coinTransform.localScale.x, position.y + coinTransform.localScale.y, position.z + coinTransform.localScale.z);
        //coinTransform.position = spawnPos;
        //coin.used = false;
        //coin.enabled = true;
        //coinQueue.Enqueue(coin);
        tCoin.Enqueue(coin);
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
