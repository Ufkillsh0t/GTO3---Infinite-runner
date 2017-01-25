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
    public class Spawn
    {
        private SpawnedObjectType spawnedObjectType = SpawnedObjectType.Coin;
        public GameObject gameObject;
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
    public Spawn[] platforms;
    public int amountOfCoins = 70;
    public Spawn[] coinSpawns;
    public int amountOfItems = 9;
    public Spawn[] itemSpawns;
    public int amountOfObstacles = 18;
    public Spawn[] obstacleSpawns;
    private PlayerScript player;
    private bool first = true;
    private int lastSpawnedItem = 7;

    private TCollection<CoinObject> tCoin;
    private TCollection<ItemObject> tItem;
    private TCollection<ObstacleObject> tObstacle;

    // Use this for initialization
    void Start()
    {
        player = PlayerScript.Instance;
        InstantiateCoins();
        InstantiateItems();
        InstantiateObstacles();
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
                        platformQueue.Enqueue(Instantiate(InstantiateRandomSpawnObject(platforms)).GetComponent<PlatformScript>());
                    }
                    platformQueues[floor].platformQueue = platformQueue;
                    platformQueues[floor].platformNextPosition = platformQueues[floor].platformStartPosition;
                    for (int i = 0; i < platformQueues[floor].numberOfPlatforms; i++)
                    {
                        Recycle(platformQueues[floor], floor);
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
                GameObject co = InstantiateRandomSpawnObject(coinSpawns);
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

    private void InstantiateItems()
    {
        if (itemSpawns.Length > 0)
        {
            tItem = new TCollection<ItemObject>(amountOfItems);
            for (int i = 0; i < amountOfItems; i++)
            {
                GameObject co = InstantiateRandomSpawnObject(itemSpawns);
                if (co != null)
                {
                    tItem.Enqueue(Instantiate(co).GetComponent<ItemObject>());
                }
                else
                {
                    Debug.LogError("ItemObject is null!");
                }
            }
        }
        else
        {
            Debug.LogError("Atleast one item object is needed!");
        }
    }

    private void InstantiateObstacles()
    {
        if (obstacleSpawns.Length > 0)
        {
            tObstacle = new TCollection<ObstacleObject>(amountOfObstacles);
            for (int i = 0; i < amountOfObstacles; i++)
            {
                GameObject co = InstantiateRandomSpawnObject(obstacleSpawns);
                if (co != null)
                {
                    tObstacle.Enqueue(Instantiate(co).GetComponent<ObstacleObject>());
                }
                else
                {
                    Debug.LogError("ObstacleObject is null!");
                }
            }
        }
        else
        {
            Debug.LogError("Atleast one item object is needed!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < platformQueues.Length; i++)//welke floor het platform op dit moment is.
        {
            if (platformQueues[i].platformQueue.Peek().transform.localPosition.x + platformRecycleOffset < player.transform.position.x)
            {
                Recycle(platformQueues[i], i);
            }
        }
    }

    private void Recycle(Floor floor, int floorIndex)
    {
        PlatformScript platformScript = floor.platformQueue.Dequeue();
        platformScript.SetFloor(floorIndex);
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
            if (platformScript.movementAxis == MovementAxis.None)
                SpawnObjects(platformScript.GenerateSpawnObjectTypesArray(), platformScript);
        }
        platformScript.renderer.material.color = Color.HSVToRGB(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
        Vector3 position = floor.platformNextPosition;
        position.x += platformScript.transform.localScale.x * 0.5f;
        position.y += platformScript.transform.localScale.y * 0.5f;
        //position.z += scale.z * 0.5f;

        platformScript.curLastItemPlatform = lastSpawnedItem;
        lastSpawnedItem++;

        if (first && player != null)
        {
            first = false;
            player.transform.position = new Vector3(platformScript.transform.position.x,
                platformScript.transform.position.y + platformScript.transform.localScale.y,
                platformScript.transform.position.z);
        }

        floor.platformNextPosition += new Vector3(
            UnityEngine.Random.Range(floor.platformMinGap.x, floor.platformMaxGap.x) + platformScript.transform.localScale.x + platformScript.GetMinMovement(MovementAxis.X),
            UnityEngine.Random.Range(floor.platformMinGap.y, floor.platformMaxGap.y),
            UnityEngine.Random.Range(floor.platformMinGap.z, floor.platformMaxGap.z));
        floor.platformQueue.Enqueue(platformScript);

        //TODO: ook voor X invoeren zodat de platforms niet te ver uit elkaar kunnen gaan.
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
                        SpawnItem(ps, x, y);
                        break;
                    case SpawnedObjectType.Obstacle:
                        SpawnObstacle(ps, x, y);
                        break;
                }
            }
        }
    }

    private GameObject InstantiateRandomSpawnObject(Spawn[] spawnArray)
    {
        float spawnFloat = UnityEngine.Random.Range(minSpawnObjectChance, maxSpawnObjectChance);
        foreach (Spawn sp in spawnArray)
        {
            if (spawnFloat <= sp.spawnObjectChance)
            {
                return sp.gameObject;
            }
        }
        return null;
    }

    private void SpawnCoin(PlatformScript ps, int x, int y)
    {
        CoinObject coin = tCoin.DequeueMax();
        coin.transform.parent = null;
        ps.SpawnObject(coin, x, y);
        tCoin.Enqueue(coin);
    }

    private void SpawnItem(PlatformScript ps, int x, int y)
    {
        Debug.Log("Magnet spawned");
        lastSpawnedItem = 0;
        ItemObject item = tItem.DequeueMax();
        item.transform.parent = null;
        ps.SpawnObject(item, x, y);
        tItem.Enqueue(item);
    }

    private void SpawnObstacle(PlatformScript ps, int x, int y)
    {
        Debug.Log("Obstacle spawned!");
        ObstacleObject obs = tObstacle.DequeueMax();
        obs.transform.parent = null;
        ps.SpawnObject(obs, x, y);
        tObstacle.Enqueue(obs);
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

    public PlatformScript GetNextPlatform(Vector3 pos, NextFloor nf = NextFloor.Same)
    {
        int curFloor = GetClosestFloor(pos);
        switch (nf)
        {
            case NextFloor.Lower:
                int lowerFloor = (curFloor < (platformQueues.Length - 1)) ? ++curFloor : platformQueues.Length - 1;
                Debug.Log(lowerFloor);
                return GetNextPlatform(lowerFloor);
            case NextFloor.Upper:
                int upperFloor = (curFloor > 0) ? --curFloor : 0;
                Debug.Log(upperFloor);
                return GetNextPlatform(upperFloor);
            case NextFloor.Same:
                return GetNextPlatform(curFloor);
        }
        return null;
    }

    public int GetClosestFloor(Vector3 pos)
    {
        int floor = int.MaxValue;
        float minDifference = float.MaxValue;
        for (int i = 0; i < platformQueues.Length; i++)
        {
            float difference = Mathf.Abs(platformQueues[i].platformMinY - pos.y);
            float difference2 = Mathf.Abs(platformQueues[i].platformMaxY - pos.y);

            if (difference > difference2 && minDifference > difference2)
            {
                minDifference = difference2;
                floor = i;
            }
            else if (minDifference > difference)
            {
                minDifference = difference;
                floor = i;
            }
        }
        return floor;
    }

    public PlatformScript GetNextPlatform(int floor)
    {
        if (floor < platformQueues.Length && floor >= 0)
        {
            PlatformScript[] platformScripts = platformQueues[floor].platformQueue.ToArray();
            int nextPlatform = (platformScripts.Length >= 1) ? 1 : 0;
            return platformScripts[nextPlatform];
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Pickup(float range)
    {
        CoinObject coin = tCoin.DequeueMin(range);
        if (coin != null)
        {
            coin.PickUp();
            tCoin.Enqueue(coin);
        }
    }
}
