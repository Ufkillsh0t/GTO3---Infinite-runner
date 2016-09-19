using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{

    private Vector3 platformNextPosition;
    public int numberOfPlatforms;
    private Queue<Transform> platformQueue;
    public Transform platformPrefab;
    public float platformRecycleOffset;
    public Vector3 platformStartPosition;
    public Vector3 platformMinSize, platformMaxSize, platformMinGap, platformMaxGap;
    public float platformMinY, platformMaxY;

    public Transform coinPrefab;
    public int numberOfCoins;
    public Vector3 coinHidePosition;
    public Vector3 coinGridSize;
    public int chancePerGrid;
    private Queue<Transform> coinQueue;

    private GameObject player;



    // Use this for initialization
    void Start()
    {
        platformQueue = new Queue<Transform>(numberOfPlatforms);
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            platformQueue.Enqueue((Transform)Instantiate(platformPrefab));
        }

        coinQueue = new Queue<Transform>(numberOfCoins);
        coinPrefab.position = coinHidePosition;
        for (int i = 0; i < numberOfCoins; i++)
        {
            coinQueue.Enqueue((Transform)Instantiate(coinPrefab));
        }

        platformNextPosition = platformStartPosition;
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            Recycle();
        }
        player = GameObject.FindGameObjectWithTag("Player");
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

        int grids = GetGrids(scale);
        Debug.Log(grids);

        Vector3 position = platformNextPosition;
        position.x += scale.x * 0.5f;
        position.y += scale.y * 0.5f;

        Transform platform = platformQueue.Dequeue();
        platform.localScale = scale;
        platform.localPosition = position;
        platform.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        platformQueue.Enqueue(platform);
        
        SpawnCoinsPlatform(position, scale);

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

    private void SpawnCoinsPlatform(Vector3 position, Vector3 scale)
    {
        float maxGridX = scale.x - coinGridSize.x;
        float maxGridZ = scale.z - coinGridSize.z;
        for (float x = 0; x < maxGridX; x += coinGridSize.x)
        {
            for (float z = 0; z < maxGridZ; z += coinGridSize.z)
            {
                    Vector3 spawnpos = new Vector3(position.x + x, position.y + scale.y, position.z + z);
                    SpawnCoin(spawnpos); /*
                int range = Random.Range(0, chancePerGrid);
                if (range == 1)
                {
                }*/
            }
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        Transform coin = coinQueue.Dequeue();
        Vector3 spawnPos = new Vector3(position.x - (coinGridSize.x / 2), position.y + coin.localScale.y, position.z - (coinGridSize.z / 2));
        coin.position = spawnPos;
        coinQueue.Enqueue(coin);
    }

    public int GetGrids(Vector3 scale)
    {
        int grids = 0;
        float maxGridX = scale.x - coinGridSize.x;
        float maxGridZ = scale.z - coinGridSize.z;
        for (float x = 0; x < maxGridX; x += coinGridSize.x)
        {
            grids++;
            for(float z = 0; z < maxGridZ; z += coinGridSize.z)
            {
                grids++;
            }
        }
        return grids;
    }
}
