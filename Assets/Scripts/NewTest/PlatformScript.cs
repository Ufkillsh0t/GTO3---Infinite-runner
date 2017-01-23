using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    //TODO: Later misschien deze klasse zonder ref maken en kijken naar performance verschillen, of vragen aan leraar wat het beste is.

    [Serializable]
    public struct SpawnChance
    {
        public SpawnObject spawnObject;
        [Range(minSpawnObjectChance, maxSpawnObjectChance)]
        public float spawnChance;
    }

    [Serializable]
    public struct SpawnObjectTypeChance
    {
        public SpawnedObjectType spawnedObjectType;
        [Range(minSpawnedObjectTypeChance, maxSpawnedObjectTypeChance)]
        public float spawnChance;
        public SpawnChance[] spawnChances;
    }

    public SpawnableObject[,] platformObjects;
    public Vector2 gridSize = Vector2.one;
    public Vector2 sectionSize = new Vector2(2, 1);
    public const float minSpawnObjectChance = 0f;
    public const float maxSpawnObjectChance = 100f;
    public const float minSpawnedObjectTypeChance = 0f;
    public const float maxSpawnedObjectTypeChance = 100f;
    [Range(0, 6)]
    public int maxObstaclesPlatform = 2;
    [Range(0, 6)]
    public int minObstacleDistance = 2;
    public SpawnAlignment obstacleAlignment = SpawnAlignment.YAxis; //alignment van obstakels.
    public SpawnAlignment itemAlignment = SpawnAlignment.None;
    public SpawnAlignment defaultAlignment = SpawnAlignment.None;
    public PlacementType placementType = PlacementType.X;
    [Range(2, 12)]
    public int itemPlatformDistance = 6;
    private int curLastItemPlatform = 8;
    [Range(2, 5)]
    public int itemsGridWhenNotOne = 2;
    [Range(2, 7)]
    public int itemGridDistance = 2;
    public bool oneItemPlatform = true;
    public bool ignoreMaxOnFill = true;
    public bool centerNextToNone = true; //Centers the object if possible (When the object next to it is non existent for example)
    public bool placeNextToItemOrObstacle = true;
    public bool checkItemPlatformDistance = true;
    public SpawnType spawnType;
    public SpawnObjectTypeChance[] spawnObjectTypeChances;
    public Renderer renderer;

    // Use this for initialization
    void Awake()
    {
        renderer = this.GetComponent<Renderer>();
        /*
        Resize(5f, 8f, 1f, 1f, 2f, 3f);
        SpawnedObjectType[,] test = GenerateSpawnObjectTypesArray();
        Debug.Log("testeroni");*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Spawns a object on the rightlocation compared to this object.
    /// </summary>
    /// <param name="spawnableObject">The object you want to spawn on this platform.</param>
    /// <param name="x">the x grid</param>
    /// <param name="y">the y grid</param>
    public void SpawnObject(SpawnableObject spawnableObject, int x, int y)
    {
        float spawnDifx = transform.localPosition.x + (spawnableObject.transform.localScale.x);
        float spawnDifz = transform.localPosition.z + (spawnableObject.transform.localScale.z);

        float spawnPointX = spawnDifx + x;
        float spawnPointY = transform.localPosition.x + transform.localScale.y;
        float spawnPointZ = spawnDifz + y;

        spawnableObject.transform.position = new Vector3(spawnPointX, spawnPointY, spawnPointZ);
        spawnableObject.used = false;
        spawnableObject.Show();
    }

    /// <summary>
    /// Moves and resizes the platform.
    /// </summary>
    /// <param name="minX">Minimum x scale.</param>
    /// <param name="maxX">Maximum x scale.</param>
    /// <param name="minY">Minimum y scale.</param>
    /// <param name="maxY">Maximum y scale.</param>
    /// <param name="minZ">Minimum z scale.</param>
    /// <param name="maxZ">Maximum z scale.</param>
    /// <param name="postion">The position you want to move this platform to.</param>
    public void MoveResize(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Vector3 postion)
    {
        Resize(minX, maxX, minY, maxY, minZ, maxZ);
        transform.position = postion;
    }

    /// <summary>
    /// Resizes the object to a random size between the min an max value for the x,y and z scale.
    /// </summary>
    /// <param name="minX">Minimum x scale.</param>
    /// <param name="maxX">Maximum x scale.</param>
    /// <param name="minY">Minimum y scale.</param>
    /// <param name="maxY">Maximum y scale.</param>
    /// <param name="minZ">Minimum z scale.</param>
    /// <param name="maxZ">Maximum z scale.</param>
    public void Resize(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        Resize(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
    }

    /// <summary>
    /// Resizes the object to the size specified by the x,y and z.
    /// </summary>
    /// <param name="x">The x scale</param>
    /// <param name="y">The y scale</param>
    /// <param name="z">the z scale</param>
    public void Resize(float x, float y, float z)
    {
        Vector3 scale = new Vector3(x, y, z);
        Resize(scale);
    }

    /// <summary>
    /// Resizes this object with the given scale.
    /// </summary>
    /// <param name="scale">The scale this object needs to be.</param>
    public void Resize(Vector3 scale)
    {
        transform.localScale = scale;
        platformObjects = new SpawnableObject[GetGridsX(), GetGridsY()];
    }

    /// <summary>
    /// Gets the amount of grids on the x-axis.
    /// </summary>
    /// <returns>The amount of grids on the x-axis</returns>
    private int GetGridsX()
    {
        int grids = 0;
        float xSize = transform.localScale.x;
        while (xSize > gridSize.x)
        {
            xSize -= gridSize.x;
            grids++;
        }
        return grids;
    }

    /// <summary>
    /// Gets the amount of grids on the z-axis.
    /// </summary>
    /// <returns>The amount of grids on the z-axis</returns>
    private int GetGridsY()
    {
        int grids = 0;
        float ySize = transform.localScale.z;
        while (ySize > gridSize.y)
        {
            ySize -= gridSize.y;
            grids++;
        }
        return grids;
    }

    /// <summary>
    /// Generates an array with random objecttypes and where to spawn them.
    /// </summary>
    /// <returns>Randomized ObjectType multidimensional array.</returns>
    public SpawnedObjectType[,] GenerateSpawnObjectTypesArray()
    {
        switch (spawnType) //TODO: veranderen naar een delegate met een call naar een array voor mooiheid.
        {
            case SpawnType.Platform:
                return GenerateSpawnObjectTypesArrayPlatform();
            case SpawnType.LineX:
                return GenerateSpawnObjectTypesArrayLineX();
            case SpawnType.LineY:
                return GenerateSpawnObjectTypesArrayLineY();
            case SpawnType.Random:
                return GenerateSpawnObjectTypesArrayRandom();
            case SpawnType.RandomLineX:
                return GenerateSpawnObjectTypesArrayRandomLineX();
            case SpawnType.RandomLineY:
                return GenerateSpawnObjectTypesArrayRandomLineY();
            case SpawnType.Sectioned:
                return GenerateSpawnObjectTypesArraySectioned();
            case SpawnType.Single:
                return GenerateSpawnObjectTypesArraySingle();
        }

        return null;
    }

    #region GenerationMethods
    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array full of one object.
    /// There is offcourse a limit on obstacles.
    /// </summary>
    /// <returns>Gives back a SpawnedObjectType multidimensional array full of one object.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayPlatform()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        SpawnedObjectType sot = GetRandomSpawnObjectType(); //optimaliseren met verbeterde line code.
        if (curLastItemPlatform <= itemPlatformDistance)
        {
            while (sot == SpawnedObjectType.Item) sot = GetRandomSpawnObjectType();
        }

        switch (sot)
        {
            case SpawnedObjectType.Item:
                if (oneItemPlatform)
                {
                    PlaceOneSpawnedObjectTypeRandomly(ref spawns, sot);
                }
                else
                {
                    PlaceObjectsRandomly(ref spawns, sot, itemGridDistance, itemsGridWhenNotOne, itemAlignment, placementType);
                }
                break;
            case SpawnedObjectType.Coin:
                FillSpawnedObjectTypeArray(ref spawns, sot);
                break;
            case SpawnedObjectType.None:
                FillSpawnedObjectTypeArray(ref spawns, sot);
                break;
            case SpawnedObjectType.Obstacle:
                PlaceObjectsRandomly(ref spawns, sot, minObstacleDistance, maxObstaclesPlatform, obstacleAlignment, placementType);
                break;
        }

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each x-axis.
    /// Doesn't check for distances between objects if you want to check for it please use on of the RandomLine functions.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each x-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayLineX() //Perhaps: fill niet ignoren op deze 
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        bool maxItems = false;
        bool maxObstacles = false;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;
        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            objectCount = 0;
            SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles);
            int maxObjectCount = GetMaxObjectCount(sot);

            for (int y = 0; y < spawns.GetLength(1); y++)
            {
                if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                {
                    PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, x, y);
                    CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                }
            }
        }

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each z-axis.
    /// Doesn't check for distances between objects if you want to check for it please use on of the RandomLine functions.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each z-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayLineY()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        bool maxItems = false;
        bool maxObstacles = false;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;
        for (int y = 0; y < spawns.GetLength(1); y++)
        {
            objectCount = 0;
            SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles);
            int maxObjectCount = GetMaxObjectCount(sot);

            for (int x = 0; x < spawns.GetLength(0); x++)
            {
                if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                {
                    PlaceOnRightAlignmentY(ref spawns, ref objectCount, sot, x, y, y);
                    CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                }
            }
        }

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with random SpawnedObjectTypes on.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array based on sections.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandom()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        bool maxItems = false;
        bool maxObstacles = false;

        Vector2 lastItemPos = Vector2.down;
        Vector2 lastObstaclePos = Vector2.down;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;
        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            objectCount = 0;
            for (int y = 0; y < spawns.GetLength(1); y++)
            {
                SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles, x, y, lastItemPos, lastObstaclePos, checkItemPlatformDistance, defaultAlignment);
                int maxObjectCount = GetMaxObjectCount(sot);

                if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                {
                    PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, x, y);
                    switch (sot)
                    {
                        case SpawnedObjectType.Item:
                            lastItemPos = new Vector2(x, y);
                            break;
                        case SpawnedObjectType.Obstacle:
                            lastObstaclePos = new Vector2(x, y);
                            break;
                    }
                    CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                }
            }
        }

        return spawns;

    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with randomObjects sorted line based on the x-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with randomObjects sorted line based on the x-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandomLineX()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        bool maxItems = false;
        bool maxObstacles = false;

        Vector2 lastItemPos = Vector2.down;
        Vector2 lastObstaclePos = Vector2.down;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;
        int y = 0;
        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles, x, y, lastItemPos, lastObstaclePos, checkItemPlatformDistance, defaultAlignment);
            int maxObjectCount = GetMaxObjectCount(sot);
            objectCount = 0;

            for (y = 0; y < spawns.GetLength(1); y++)
            {
                if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                {
                    PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, x, y);
                    switch (sot)
                    {
                        case SpawnedObjectType.Item:
                            lastItemPos = new Vector2(x, y);
                            break;
                        case SpawnedObjectType.Obstacle:
                            lastObstaclePos = new Vector2(x, y);
                            break;
                    }
                    CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                }
            }
        }

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with randomObjects sorted line based on the z-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with randomObjects sorted line based on the z-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandomLineY()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        bool maxItems = false;
        bool maxObstacles = false;

        Vector2 lastItemPos = Vector2.down;
        Vector2 lastObstaclePos = Vector2.down;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;
        int x = 0;
        for (int y = 0; y < spawns.GetLength(1); y++)
        {
            SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles, x, y, lastItemPos, lastObstaclePos, checkItemPlatformDistance, defaultAlignment);
            int maxObjectCount = GetMaxObjectCount(sot);
            objectCount = 0;

            for (x = 0; x < spawns.GetLength(0); x++)
            {
                if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                {
                    PlaceOnRightAlignmentY(ref spawns, ref objectCount, sot, x, y, y);
                    switch (sot)
                    {
                        case SpawnedObjectType.Item:
                            lastItemPos = new Vector2(x, y);
                            break;
                        case SpawnedObjectType.Obstacle:
                            lastObstaclePos = new Vector2(x, y);
                            break;
                    }
                    CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                }
            }
        }

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one object that isn't none.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one object that isn't none.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArraySingle()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        PlaceOneSpawnedObjectTypeRandomly(ref spawns, GetRandomSpawnObjectType());

        return spawns;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array based on sections.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array based on sections.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArraySectioned()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        int secX = (int)sectionSize.x;
        int secY = (int)sectionSize.y;
        int xSize = Mathf.CeilToInt((float)spawns.GetLength(0) / secX);
        int ySize = Mathf.CeilToInt((float)spawns.GetLength(1) / secY);

        bool maxItems = false;
        bool maxObstacles = false;

        Vector2 lastItemPos = Vector2.down;
        Vector2 lastObstaclePos = Vector2.down;

        int objectCount = 0;
        int itemCount = 0;
        int obstacleCount = 0;

        for (int xSec = 0; xSec < xSize; xSec++)
        {
            for (int ySec = 0; ySec < ySize; ySec++)
            {
                SpawnedObjectType sot = GetRandomSpawnObjectType(maxItems, maxObstacles, xSec, ySec, lastItemPos, lastObstaclePos, checkItemPlatformDistance, defaultAlignment);
                int maxObjectCount = GetMaxObjectCount(sot);
                objectCount = 0;

                int secXStart = (xSec != 0) ? (xSec * secX) : 0;
                int secYStart = (ySec != 0) ? (ySec * secY) : 0;
                int secXLength = (secXStart < spawns.GetLength(0)) ?
                    (((secXStart + secX) < spawns.GetLength(0)) ? secXStart + secX : spawns.GetLength(0))
                    : secXStart;
                int secYLength = (secYStart < spawns.GetLength(1)) ?
                    (((secYStart + secY) < spawns.GetLength(1)) ? secYStart + secY : spawns.GetLength(1))
                    : secYStart;

                for (int x = secXStart; x < secXLength; x++)
                {
                    for (int y = secYStart; y < secYLength; y++)
                    {
                        if (ignoreMaxOnFill || objectCount < maxObjectCount || maxObjectCount == -1)
                        {
                            PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, x, y);
                            switch (sot)
                            {
                                case SpawnedObjectType.Item:
                                    lastItemPos = new Vector2(x, y);
                                    break;
                                case SpawnedObjectType.Obstacle:
                                    lastObstaclePos = new Vector2(x, y);
                                    break;
                            }
                            CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, ignoreMaxOnFill);
                        }
                    }
                }

            }
        }

        return spawns;
    }
    #endregion

    #region PlaceFillMethods
    /// <summary>
    /// Places a spawned object type on the given axises in the given array.
    /// </summary>
    /// <param name="spawns">The array in which you want to place the spawnedObjectTypes.</param>
    /// <param name="sot">The SpawnedObjectType you want to place.</param>
    /// <param name="x">The x-axis you want to place it on.</param>
    /// <param name="z">The z-axis you want to place it on.</param>
    private void PlaceSpawnedObjectTypeInArray(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int x, int y)
    {
#if UNITY_EDITOR
        if (x > spawns.GetLength(0) || y > spawns.GetLength(1)) Debug.Log("Out of bound exception for: " + sot + " on the following axisses x:" + x + " z:" + y);
#endif
        spawns[x, y] = sot;
    }

    /// <summary>
    /// Fills the given SpawnedObjectType array fully with one objectType.
    /// </summary>
    /// <param name="spawns">The SpawnedObjectType array you want to add objecttypes to.</param>
    /// <param name="sot">The spawnedObjectType you want to fill the array with.</param>
    private void FillSpawnedObjectTypeArray(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot)
    {
        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            for (int y = 0; y < spawns.GetLength(1); y++)
            {
                PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, y);
            }
        }
    }


    /// <summary>
    /// Places a item in a random place in the array.
    /// </summary>
    /// <param name="spawns">The spawn array</param>
    /// <param name="sot">The spawnObjectType you want to place.</param>
    private void PlaceOneSpawnedObjectTypeRandomly(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot)
    {
        int xPos = (centerNextToNone && (spawns.GetLength(0) % 2 == 1)) ? (spawns.GetLength(0) + (spawns.GetLength(0) % 2)) / 2 : UnityEngine.Random.Range(0, spawns.GetLength(0)); //Gets the center grid if center is true else it gets a random value on the x axis of the grid
        int yPos = UnityEngine.Random.Range(0, spawns.GetLength(1));
        for (int y = 0; y < spawns.GetLength(1); y++)
        {
            for (int x = 0; x < spawns.GetLength(0); x++)
            {
                if (x == xPos && y == yPos)
                {
                    PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, y);
                }
                else
                {
                    PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, y);
                }
            }
        }
    }

    /// <summary>
    /// Places the given SpawnedObjectType randomly on the platform based upon the given SpawnAlignment, the distance and the max amount of objects.
    /// </summary>
    /// <param name="spawns">The array in which you want to have the spawnedObjectTypes. This method only fills the array up to 2 dimensions.</param>
    /// <param name="sot">The object you want to spawn randomly across the platform.</param>
    /// <param name="distance">The minimum distance between objects.</param>
    /// <param name="maxObjects">The maximum amount of objects you want to spawn.</param>
    /// <param name="alignment"></param>
    /// <param name="placementType">This will change the effect of the placement within the loop, based on grid axis</param>
    private void PlaceObjectsRandomly(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maxObjects, SpawnAlignment alignment = SpawnAlignment.None, PlacementType placementType = PlacementType.X)
    {
        switch (placementType)
        {
            case PlacementType.X:
                PlaceObjectsRandomlyX(ref spawns, sot, distance, maxObjects, alignment);
                break;
            case PlacementType.Y:
                PlaceObjectsRandomlyY(ref spawns, sot, distance, maxObjects, alignment);
                break;
        }
    }

    /// <summary>
    /// Places the given SpawnedObjectType randomly on the platform based upon the given SpawnAlignment, the distance and the max amount of objects.
    /// </summary>
    /// <param name="spawns">The array in which you want to have the spawnedObjectTypes. This method only fills the array up to 2 dimensions.</param>
    /// <param name="sot">The object you want to spawn randomly across the platform.</param>
    /// <param name="distance">The minimum grid distance between objects.</param>
    /// <param name="maximumObjects">The maximum amount of objects you want to spawn.</param>
    /// <param name="alignment"></param>
    private void PlaceObjectsRandomlyX(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maximumObjects, SpawnAlignment alignment = SpawnAlignment.None)
    {
        int xPos = GetXPos(alignment, spawns.GetLength(0));
        int yPos = GetYPos(alignment, spawns.GetLength(1));

        int objectCount = 0;
        bool canSpawn = true;
        int maxObjects = maximumObjects;
        int gridDistance = distance + 1;

        if (sot == SpawnedObjectType.Item && oneItemPlatform) maxObjects = 1;

        for (int y = 0; y < spawns.GetLength(1); y++)
        {
            if (!canSpawn) return;
            for (int x = 0; x < spawns.GetLength(0); x++)
            {
                if (!canSpawn) return;
                switch (alignment)
                {
                    case SpawnAlignment.None:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                        {
                            yPos = GetYPos(alignment, spawns.GetLength(1), y); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om hier te gebruiken.
                            xPos = (yPos > y) ? TRandom.GetRandomValue(0, spawns.GetLength(0), x, gridDistance) : GetXPos(alignment, spawns.GetLength(0), x + gridDistance); //GetXPos(alignment, spawns.GetLength(1), x + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om te gebruiken.
                        }
                        if (yPos == -1 || xPos == -1) canSpawn = false;
                        break;
                    case SpawnAlignment.XAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                            yPos = GetYPos(alignment, spawns.GetLength(1), y + gridDistance); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om hier te gebruiken.
                        if (yPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.XAxisFill:
                        if (ignoreMaxOnFill || objectCount < maxObjects)
                        {
                            PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, xPos, y);
                            if (objectCount < maxObjects && y >= spawns.GetLength(1))
                                xPos = GetXPos(alignment, spawns.GetLength(0), x + gridDistance);
                        }
                        break;
                    case SpawnAlignment.YAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                            xPos = TRandom.GetRandomValue(0, spawns.GetLength(0), x, gridDistance); //GetXPos(alignment, spawns.GetLength(1), x + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om te gebruiken.
                        if (xPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.YAxisFill:
                        if (ignoreMaxOnFill || objectCount < maxObjects)
                        {
                            PlaceOnRightAlignmentY(ref spawns, ref objectCount, sot, x, y, yPos);
                            if (objectCount < maxObjects && x >= spawns.GetLength(0))
                                yPos = GetYPos(alignment, spawns.GetLength(1), y + gridDistance);
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Places the given SpawnedObjectType randomly on the platform based upon the given SpawnAlignment, the distance and the max amount of objects.
    /// </summary>
    /// <param name="spawns">The array in which you want to have the spawnedObjectTypes. This method only fills the array up to 2 dimensions.</param>
    /// <param name="sot">The object you want to spawn randomly across the platform.</param>
    /// <param name="distance">The minimum distance between objects.</param>
    /// <param name="maximumObjects">The maximum amount of objects you want to spawn.</param>
    /// <param name="alignment"></param>
    private void PlaceObjectsRandomlyY(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maximumObjects, SpawnAlignment alignment = SpawnAlignment.None)
    {
        int xPos = GetXPos(alignment, spawns.GetLength(0));
        int yPos = GetYPos(alignment, spawns.GetLength(1));

        int objectCount = 0;
        bool canSpawn = true;
        int maxObjects = maximumObjects;
        int gridDistance = distance + 1;

        if (sot == SpawnedObjectType.Item && oneItemPlatform) maxObjects = 1;

        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            if (!canSpawn) return;
            for (int y = 0; y < spawns.GetLength(1); y++)
            {
                if (!canSpawn) return;
                switch (alignment)
                {
                    case SpawnAlignment.None:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                        {
                            xPos = GetXPos(alignment, spawns.GetLength(0), x); //verschil met de andere method zit hier.
                            yPos = (xPos > x) ? TRandom.GetRandomValue(0, spawns.GetLength(0), y, gridDistance) : GetYPos(alignment, spawns.GetLength(1), y + gridDistance); //verschil met de andere method zit hier.
                        }
                        if (yPos == -1 || xPos == -1) canSpawn = false;
                        break;
                    case SpawnAlignment.XAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                            yPos = TRandom.GetRandomValue(0, spawns.GetLength(0), y, gridDistance); //verschil met de andere method zit hier.
                        if (yPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.XAxisFill:
                        if (ignoreMaxOnFill || objectCount < maxObjects)
                        {
                            PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, xPos, y);
                            if (objectCount < maxObjects && y >= spawns.GetLength(1))
                                xPos = GetXPos(alignment, spawns.GetLength(0), x + gridDistance);
                        }
                        break;
                    case SpawnAlignment.YAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, y, yPos))
                            xPos = GetXPos(alignment, spawns.GetLength(0), x + gridDistance); //verschil met de andere method zit hier.
                        if (xPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.YAxisFill:
                        if (ignoreMaxOnFill || objectCount < maxObjects)
                        {
                            PlaceOnRightAlignmentY(ref spawns, ref objectCount, sot, x, y, yPos);
                            if (objectCount < maxObjects && x >= spawns.GetLength(0))
                                yPos = GetYPos(alignment, spawns.GetLength(1), y + gridDistance);
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Places the object if the given x equals the xPos.
    /// </summary>
    /// <param name="spawns">The spawnObjectType array.</param>
    /// <param name="objectCount">The current amount of objects placed.</param>
    /// <param name="sot">The spawnObjectType you want to place.</param>
    /// <param name="x">The current x coordinate you are checking.</param>
    /// <param name="xPos">The x coordinate you want to place it on.</param>
    /// <param name="z">The current z coordinate you are checking.</param>
    /// <param name="defaultObjectType">The default object you want to spawn if x is not equal to xpos</param>
    private void PlaceOnRightAlignmentX(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int xPos, int y, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (x == xPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, y);
            objectCount++;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, defaultObjectType, x, y);
        }
    }

    /// <summary>
    /// Places the object if the given x equals the xPos.
    /// </summary>
    /// <param name="spawns">The spawnObjectType array.</param>
    /// <param name="objectCount">The current amount of objects placed.</param>
    /// <param name="sot">The spawnObjectType you want to place.</param>
    /// <param name="x">The current x coordinate you are checking.</param>
    /// <param name="zPos">The z coordinate you want to place it on.</param>
    /// <param name="z">The current z coordinate you are checking.</param>
    /// <param name="defaultObjectType">The default object you want to spawn if z is not equal to zpos</param>
    private void PlaceOnRightAlignmentY(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int y, int yPos, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (y == yPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, y);
            objectCount++;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, y);
        }
    }

    /// <summary>
    /// Places the object if the given x equals the xPos.
    /// </summary>
    /// <param name="spawns">The spawnObjectType array.</param>
    /// <param name="objectCount">The current amount of objects placed.</param>
    /// <param name="sot">The spawnObjectType you want to place.</param>
    /// <param name="x">The current x coordinate you are checking.</param>
    /// <param name="xPos">The x coordinate you want to place it on.</param>
    /// <param name="z">The current z coordinate you are checking.</param>
    /// <param name="zPos">The z coordinate you want to place it on.</param>
    /// <param name="defaultObjectType">The default object you want to spawn if x is not equal to xpos and z is not equal to zpos</param>
    /// <returns>Return true when it has placed an object in the array.</returns>
    private bool PlaceOnRightAlignment(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int xPos, int y, int yPos, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (x == xPos && y == yPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, y);
            objectCount++;
            return true;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, y);
            return false;
        }
    }
    #endregion

    #region CheckSetSpawnedObjectTypeMethods
    /// <summary>
    /// Checks wether a item or obstacle may spawn next, or generates another object that may spawn.
    /// </summary>
    /// <param name="sot">The object you want to spawn.</param>
    /// <param name="itemCount">the current amount of items.</param>
    /// <param name="obstacleCount">the current amount of obstacles.</param>
    /// <param name="objectCount">the current amount of objects on the axis.</param>
    /// <param name="maxObjectCount">the maximum of objects allowed.</param>
    /// <param name="maxItems">the maximum of items allowed.</param>
    /// <param name="maxObstacles">the maximum of obstacles allowed.</param>
    private void CheckIfItMaySpawn(ref SpawnedObjectType sot, ref int itemCount, ref int obstacleCount, ref int objectCount, ref int maxObjectCount, ref bool maxItems, ref bool maxObstacles, bool ignoreMaxOnFill = false)
    {
        //GetRandomSpawnObjectType(maxItems, maxObstacles, 0, 0, Vector2.zero, Vector2.zero, true, SpawnAlignment.XAxisFill);
        CheckIfItMaySpawn(ref sot, ref itemCount, ref obstacleCount, ref objectCount, ref maxObjectCount, ref maxItems, ref maxObstacles, 0, 0, Vector2.down, Vector2.down, ignoreMaxOnFill);
    }

    /// <summary>
    /// Checks wether a item or obstacle may spawn next, or generates another object that may spawn.
    /// </summary>
    /// <param name="sot">The object you want to spawn.</param>
    /// <param name="itemCount">the current amount of items.</param>
    /// <param name="obstacleCount">the current amount of obstacles.</param>
    /// <param name="objectCount">the current amount of objects on the axis.</param>
    /// <param name="maxObjectCount">the maximum of objects allowed.</param>
    /// <param name="maxItems">the maximum of items allowed.</param>
    /// <param name="maxObstacles">the maximum of obstacles allowed.</param>
    /// <param name="ignoreMaxOnFill">wether to ignore the fill or not</param>
    /// <param name="lastItem">the position of the lastitem placed on the platform.</param>
    /// <param name="lastObstacle">the position of the lastobstacle placed on the platform.</param>
    /// <param name="x">the x position you want to check for spawn.</param>
    /// <param name="y">the y position you want to check for spawn.</param>
    private bool CheckIfItMaySpawn(ref SpawnedObjectType sot, ref int itemCount, ref int obstacleCount, ref int objectCount, ref int maxObjectCount, ref bool maxItems, ref bool maxObstacles,
        int x, int y, Vector2 lastItem, Vector2 lastObstacle, bool ignoreMaxOnFill = false)
    {
        if (sot == SpawnedObjectType.Item)
        {
            itemCount++;
            if ((itemCount >= maxObjectCount || oneItemPlatform))
            {
                SetMaxSpawnTypeBoolToTrue(sot, ref maxItems, ref maxObstacles);
                if (!ignoreMaxOnFill || oneItemPlatform)
                {
                    sot = GetRandomSpawnObjectType(maxItems, maxObstacles, x, y, lastItem, lastObstacle, checkItemPlatformDistance, defaultAlignment);
                    maxObjectCount = GetMaxObjectCount(sot);
                    objectCount = 0;
                    return true;
                }
            }
        }
        else if (sot == SpawnedObjectType.Obstacle)
        {
            obstacleCount++;
            if (obstacleCount >= maxObjectCount)
            {
                SetMaxSpawnTypeBoolToTrue(sot, ref maxItems, ref maxObstacles);
                if (!ignoreMaxOnFill)
                {
                    sot = GetRandomSpawnObjectType(maxItems, maxObstacles, x, y, lastItem, lastObstacle, checkItemPlatformDistance, defaultAlignment);
                    maxObjectCount = GetMaxObjectCount(sot);
                    objectCount = 0;
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Sets the MaxItems of MaxObstacles boolean too true if the sot is a item or obstacle.
    /// </summary>
    /// <param name="sot">The spawnedObjectType</param>
    /// <param name="maxItems">The maxItems boolean.</param>
    /// <param name="maxObstacles">The maxObstacles boolean.</param>
    private void SetMaxSpawnTypeBoolToTrue(SpawnedObjectType sot, ref bool maxItems, ref bool maxObstacles)
    {
        switch (sot)
        {
            case SpawnedObjectType.Item:
                maxItems = true;
                break;
            case SpawnedObjectType.Obstacle:
                maxObstacles = true;
                break;
        }
    }

    /// <summary>
    /// Gets the maxObjectcount of the given object.
    /// </summary>
    /// <param name="sot">The spawneObjectType you want the maxCountFor</param>
    /// <returns>The maximum amount of objects allowed on the platform for the given objecttype.</returns>
    private int GetMaxObjectCount(SpawnedObjectType sot)
    {
        switch (sot)
        {
            case SpawnedObjectType.Item:
                return itemsGridWhenNotOne;
            case SpawnedObjectType.Obstacle:
                return maxObstaclesPlatform;
            default:
                return -1;
        }
    }
    #endregion

    #region RandomMethods
    /// <summary>
    /// Gets a random int based upon the spawnAlignment of the x-axis.
    /// </summary>
    /// <param name="alignment">The spawnAlignment</param>
    /// <param name="length">The maximum value it may return.</param>
    /// <param name="min">The minimum value you want to have, is by default always 0</param>
    /// <returns>A integer between 0 and the maximum value or -1 depending on the spawnaligment</returns>
    /// <exception cref="Exception">Throws a exception when the value is lower than 0</exception>
    public int GetXPos(SpawnAlignment alignment, int max, int min = 0)
    {
        if (min < 0 && max < 0) throw new Exception("Value needs to be higher than 0");
        if (min > max) return -1;
        if (min == max) return max;
        switch (alignment)
        {
            case SpawnAlignment.None:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.XAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.XAxisFill:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.YAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.YAxisFill:
                return -1;
        }
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// Gets a random int based upon the spawnAlignment of the z-axis.
    /// </summary>
    /// <param name="alignment">The spawnAlignment</param>
    /// <param name="length">The maximum value it may return.</param>
    /// <param name="min">The minimum value you want to have, is by default always 0</param>
    /// <returns>A integer between 0 and the maximum value or -1 depending on the spawnaligment</returns>
    /// <exception cref="Exception">Throws a exception when the value is lower than 0</exception>
    public int GetYPos(SpawnAlignment alignment, int max, int min = 0)
    {
        if (min < 0 && max < 0) throw new Exception("Value needs to be higher than 0");
        if (min > max) return -1;
        if (min == max) return max;
        switch (alignment)
        {
            case SpawnAlignment.None:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.XAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.XAxisFill:
                return -1;
            case SpawnAlignment.YAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.YAxisFill:
                return UnityEngine.Random.Range(min, max);
        }
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// Returns a spawnedObjectType based upon the spawnchance you have given.
    /// It works the same as <see cref="GetSpawnObject(SpawnedObjectType)"/> to determine the value.
    /// </summary>
    /// <returns>A spawnedObjectType</returns>
    private SpawnedObjectType GetRandomSpawnObjectType()
    {
        float spawnFloat = UnityEngine.Random.Range(minSpawnObjectChance, maxSpawnObjectChance);
        foreach (SpawnObjectTypeChance sotc in spawnObjectTypeChances)
        {
            if (spawnFloat < sotc.spawnChance)
            {
                return sotc.spawnedObjectType;
            }
        }
        return SpawnedObjectType.None;
    }

    /// <summary>
    /// Gets a random SpawnedObjectType based on a maxitems and maxobstacles boolean.
    /// </summary>
    /// <param name="maxItems">If the amount of items has reached max.</param>
    /// <param name="maxObstacles">If the amount of obstacles has reached max.</param>
    /// <param name="checkLastItemPlatform">If it needs to check for item platform distances.</param>
    /// <returns></returns>
    private SpawnedObjectType GetRandomSpawnObjectType(bool maxItems, bool maxObstacles, SpawnAlignment alignment = SpawnAlignment.XAxisFill, bool checkLastItemPlatform = true)
    { //Der uit met dezen handel;
        return GetRandomSpawnObjectType(maxItems, maxObstacles, 0, 0, Vector2.zero, Vector2.zero, checkLastItemPlatform, alignment);
    }

    /// <summary>
    /// Gets a random SpawnedObjectType based on a maxitems and maxobstacles boolean.
    /// </summary>
    /// <param name="maxItems">If the amount of items has reached max.</param>
    /// <param name="maxObstacles">If the amount of obstacles has reached max.</param>
    /// <param name="checkLastItemPlatform">If it needs to check for item platform distances.</param>
    /// <param name="x">The current x coord.</param>
    /// <param name="y">the current y coord.</param>
    /// <param name="lastItem">The last items coords</param>
    /// <param name="lastObstacle">The last obstacles coord</param>
    /// <param name="alignment">If you chose a non fill alignment method this will check for the distance between objects.</param>
    /// <returns></returns>
    private SpawnedObjectType GetRandomSpawnObjectType(bool maxItems, bool maxObstacles, int x, int y, Vector2 lastItem, Vector2 lastObstacle, bool checkLastItemPlatform = true, SpawnAlignment alignment = SpawnAlignment.None)
    { //Der uit met dezen handel;
        int maxAmount = Enum.GetNames(typeof(SpawnedObjectType)).Length;
        bool checkDistance = !(alignment == SpawnAlignment.XAxisFill || alignment == SpawnAlignment.YAxisFill);

        int itemID = (int)SpawnedObjectType.Item;
        int obstacleID = (int)SpawnedObjectType.Obstacle;

        HashSet<int> excluded = new HashSet<int>();
        if (!maxItems && !maxObstacles)
        {
            if (checkLastItemPlatform && curLastItemPlatform <= itemPlatformDistance)
            {
                excluded.Add(itemID);
            }
            else if (checkDistance)
            {
                if (lastItem != Vector2.down && Vector2.Distance(lastItem, new Vector2(x, y)) < (itemGridDistance + 1))
                {
                    excluded.Add(itemID);
                }
                if (lastObstacle != Vector2.down && Vector2.Distance(lastObstacle, new Vector2(x, y)) < (minObstacleDistance + 1))
                {
                    excluded.Add(obstacleID);
                }
            }
        }
        else if (!maxItems && maxObstacles)
        {
            excluded.Add(obstacleID);
            if (checkLastItemPlatform && curLastItemPlatform <= itemPlatformDistance)
            {
                excluded.Add(itemID);
            }
            else if (checkDistance)
            {
                if (lastItem != Vector2.down && Vector2.Distance(lastItem, new Vector2(x, y)) < (itemGridDistance + 1))
                {
                    excluded.Add(itemID);
                }
            }
        }
        else if (maxItems && !maxObstacles)
        {
            excluded.Add(itemID);
            if (checkDistance)
            {
                if (lastObstacle != Vector2.down && Vector2.Distance(lastObstacle, new Vector2(x, y)) < (minObstacleDistance + 1))
                {
                    excluded.Add(obstacleID);
                }
            }
        }
        else
        {
            excluded.Add(itemID);
            excluded.Add(obstacleID);
        }

        SpawnedObjectType sot = GetRandomSpawnObjectType();
        if (excluded.Contains((int)sot))
        {
            return SpawnedObjectType.None;
        }
        return sot;
    }

    /*
    /// <summary>
    /// Returns an object based on the spawnchance you have given.
    /// The spawn chance is by default between 0 and 100.
    /// This method will check all SpawnObjects in spawnchance.
    /// In spawnChance you specify a value zone, which is a zone between the given value of the spawnobject and another objects lower value.
    /// For example you have 2 objects one with value 0.2f and one with 0.4f. The last objects zone will be between 0.2f and 0.4f.
    /// Note: A value of 0f has a 0% spawnrate.
    /// </summary>
    /// <param name="objectType">Of which objecttype you want to spawn a object, think of items, obstacles or coins.</param>
    /// <returns>The object that you will spawn.</returns>
    private SpawnObject GetSpawnObject(SpawnedObjectType objectType)
    {
        if (objectType == SpawnedObjectType.None) return SpawnObject.None;
        foreach (SpawnObjectTypeChance sotc in spawnObjectTypeChances)
        {
            if (sotc.spawnedObjectType == objectType)
            {
                float spawnFloat = UnityEngine.Random.Range(minSpawnObjectChance, maxSpawnObjectChance);
                foreach (SpawnChance sc in sotc.spawnChances)
                {
                    if (sc.spawnChance < spawnFloat)
                    {
                        return sc.spawnObject;
                    }
                }
            }
        }

#if UNITY_EDITOR
        Debug.Log("SpawnObject was not found for objecttype" + objectType);
#endif

        return SpawnObject.None;
    }*/
    #endregion
}
