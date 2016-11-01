using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Spawns
{
    None,
    Coin,
    Magnet
}

public enum CoinSpawn
{
    Line,
    Single
}

public class PlatformManager : MonoBehaviour
{
    private Vector3 platformNextPosition;
    public int numberOfPlatforms = 20;
    private Queue<Transform> platformQueue;
    public Transform platformPrefab;
    public float platformRecycleOffset;
    public Vector3 platformStartPosition;
    public Vector3 platformMinSize, platformMaxSize, platformMinGap, platformMaxGap;
    public float platformMinY, platformMaxY;

    public Transform coinPrefab;
    public int numberOfCoins = 75;
    public Vector3 coinHidePosition;
    public Vector3 gridSize;
    private TCollection<CoinScript> coinQueue;
    public CoinSpawn coinSpawnType = CoinSpawn.Line;

    public Transform magnetPrefab;
    public int numberOfMagnets = 4;
    public Vector3 magnetHidePosition;
    private Queue<Transform> magnetQueue;

    public int noneChance = 30;
    public int coinChance = 20;
    public int magnetChance = 100;
    private GameObject player;

    private int totalChance;


    // Use this for initialization
    void Start()
    {
        totalChance = noneChance + coinChance + magnetChance;

        gridSize = new Vector3(1, 1, 1);

        InstantiatePlatforms();
        InstantiateCoins();
        InstantiateMagnet();

        if (platformPrefab != null)
        {
            platformNextPosition = platformStartPosition;
            for (int i = 0; i < numberOfPlatforms; i++)
            {
                Recycle();
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");

    }

    private void InstantiatePlatforms()
    {
        if (platformPrefab != null)
        {
            platformQueue = new Queue<Transform>(numberOfPlatforms);
            for (int i = 0; i < numberOfPlatforms; i++)
            {
                platformQueue.Enqueue((Transform)Instantiate(platformPrefab));
            }
        }
        else
        {
            Debug.LogError("Could not generate platforms due to missing prefab!");
        }
    }

    private void InstantiateCoins()
    {
        if (coinPrefab != null)
        {
            coinQueue = new TCollection<CoinScript>(numberOfCoins);
            coinPrefab.position = coinHidePosition;
            for (int i = 0; i < numberOfCoins; i++)
            {
                coinQueue.Enqueue(Instantiate(coinPrefab).GetComponent<CoinScript>());
            }
        }
        else
        {
            Debug.Log("Coin prefab couldn't be found!");
        }
    }

    private void InstantiateMagnet()
    {
        if (magnetPrefab != null)
        {
            magnetQueue = new Queue<Transform>(numberOfMagnets);
            magnetPrefab.position = magnetHidePosition;
            for (int i = 0; i < numberOfMagnets; i++)
            {
                magnetQueue.Enqueue((Transform)Instantiate(magnetPrefab));
            }
        }
        else
        {
            Debug.Log("Magnet prefab couldn't be found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (platformQueue.Peek().localPosition.x + platformRecycleOffset < player.transform.position.x)
        {
            Recycle();
        }
    }

    private void Recycle()
    {
        Vector3 scale = new Vector3(
            Random.Range(platformMinSize.x, platformMaxSize.x),
            Random.Range(platformMinSize.y, platformMaxSize.y),
            Random.Range(platformMinSize.z, platformMaxSize.z)
            );

        /*
        int grids = GetGrids(scale);
        Debug.Log(grids);*/

        Vector3 position = platformNextPosition;
        position.x += scale.x * 0.5f;
        position.y += scale.y * 0.5f;
        //position.z += scale.z * 0.5f;

        Transform platform = platformQueue.Dequeue();
        platform.localScale = scale;
        platform.localPosition = position;
        platform.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        platformQueue.Enqueue(platform);

        SpawnGridsPlatform(position, scale);

        platformNextPosition += new Vector3(
            Random.Range(platformMinGap.x, platformMaxGap.x) + scale.x,
            Random.Range(platformMinGap.y, platformMaxGap.y),
            Random.Range(platformMinGap.z, platformMaxGap.z));

        if (platformNextPosition.y < platformMinY)
        {
            platformNextPosition.y = platformMinY + platformMaxGap.y;
        }
        else if (platformNextPosition.y > platformMaxY)
        {
            platformNextPosition.y = platformMaxY - platformMaxGap.y;
        }
    }

    private void SpawnGridsPlatform(Vector3 position, Vector3 scale)
    {
        int gridsX = GetGridsX(scale);
        int gridsZ = GetGridsZ(scale);
        float xSpawnGap = (scale.x - gridsX) / 2;
        float zSpawnGap = (scale.z - gridsZ) / 2;

        //Zet de posities juist voor gridspawning.
        position.x -= (scale.x / 2);
        position.z -= (scale.z / 2);
        position.x += xSpawnGap;
        position.z += zSpawnGap;
        position.y += scale.y;

        Spawns[,] gridSpawnArray = GetSpawns(gridsX, gridsZ);
        SpawnPlatform(gridSpawnArray, position);
    }

    private Spawns[,] GetSpawns(int gridsX, int gridsZ)
    {
        Spawns[,] gridSpawnArray = new Spawns[gridsZ, gridsX];

        for (int z = 0; z < gridSpawnArray.GetLength(0); z++)
        {
            int chance = Random.Range(0, totalChance);
            Spawns spawn = GetSpawnByChance(chance);
            bool magnetSpawned = false;

            for (int x = 0; x < gridSpawnArray.GetLength(1); x++)
            {
                if (coinSpawnType == CoinSpawn.Line && spawn == Spawns.Coin)
                {
                    gridSpawnArray[z, x] = spawn;
                }
                else if (coinSpawnType != CoinSpawn.Line && x > 0)
                {
                    chance = Random.Range(0, totalChance);
                    spawn = GetSpawnByChance(chance);
                    if (spawn == Spawns.Magnet)
                    {
                        if (magnetSpawned)
                        {
                            spawn = Spawns.None;
                        }
                        else
                        {
                            magnetSpawned = true;
                        }
                    }
                    gridSpawnArray[z, x] = spawn;
                }
                else if (spawn == Spawns.Magnet)
                {
                    if (magnetSpawned)
                    {
                        spawn = Spawns.None;
                    }
                    else
                    {
                        gridSpawnArray[z, x] = spawn;
                        magnetSpawned = true;
                    }
                }
            }
        }

        return gridSpawnArray;
    }

    private Spawns GetSpawnByChance(int chance)
    {
        if (chance <= noneChance)
        {
            return Spawns.None;
        }
        if (chance <= (noneChance + coinChance))
        {
            return Spawns.Coin;
        }
        if (chance <= (noneChance + coinChance + magnetChance))
        {
            return Spawns.Magnet;
        }
        return Spawns.None;
    }

    //private Spawns Get

    private void SpawnPlatform(Spawns[,] gridSpawnArray, Vector3 position)
    {
        for (int z = 0; z < gridSpawnArray.GetLength(0); z++)
        {
            for (int x = 0; x < gridSpawnArray.GetLength(1); x++)
            {
                Vector3 spawnpos = new Vector3(position.x + x, position.y, position.z + z);
                switch (gridSpawnArray[z, x])
                {
                    case Spawns.Coin:
                        SpawnCoin(spawnpos);
                        break;
                    case Spawns.Magnet:
                        SpawnMagnet(spawnpos);
                        break;
                }
            }
        }
    }

    private int GetGridsX(Vector3 scale)
    {
        int grids = 0;
        float xSize = scale.x;
        while (xSize > gridSize.x)
        {
            xSize -= gridSize.x;
            grids++;
        }
        return grids;
    }

    private int GetGridsZ(Vector3 scale)
    {
        int grids = 0;
        float zSize = scale.z;
        while (zSize > gridSize.z)
        {
            zSize -= gridSize.z;
            grids++;
        }
        return grids;
    }

    private void SpawnCoin(Vector3 position)
    {
        CoinScript coin = coinQueue.DequeueMax();
        Transform coinTransform = coin.transform;
        Vector3 spawnPos = new Vector3(position.x + coinTransform.localScale.x, position.y + coinTransform.localScale.y, position.z + coinTransform.localScale.z);
        coinTransform.position = spawnPos;
        coin.pickedUp = false;
        //coin.enabled = true;
        coinQueue.Enqueue(coin);
    }

    public void PickupCoin(float range)
    {
        CoinScript coin = coinQueue.DequeueMin(range);
        if (coin != null)
        {
            coin.PickUp();
            coinQueue.Enqueue(coin);
        }
    }

    private void SpawnMagnet(Vector3 position)
    {
        Transform magnet = magnetQueue.Dequeue();
        Vector3 spawnPos = new Vector3(position.x + magnet.localScale.x, position.y, position.z + magnet.localScale.z);
        magnet.position = spawnPos;
        magnetQueue.Enqueue(magnet);
    }


}
