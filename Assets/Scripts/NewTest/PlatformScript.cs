using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{

    public enum SpawnType
    {
        Platform,
        LineX,
        LineZ,
        Random,
        RandomLineX,
        RandomLineZ,
        Single,
        Sectioned
    }

    public enum SpawnAlignment
    {
        ZAxis,
        XAxis,
        ZAxisFill, //Ignores the maximum and fills the entire row.
        XAxisFill, //Ignores the maximum and fills the entire row.
        None
    }

    public enum PlacementType
    {
        X,
        Z
    }

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
    public Vector3 gridSize = Vector3.one;
    public Vector3 sectionSize = new Vector3(2, 0, 1);
    public const float minSpawnObjectChance = 0f;
    public const float maxSpawnObjectChance = 100f;
    public const float minSpawnedObjectTypeChance = 0f;
    public const float maxSpawnedObjectTypeChance = 100f;
    [Range(0,6)]
    public int maxObstaclesPlatform = 2;
    [Range(0, 6)]
    public int minObstacleDistance = 2;
    public SpawnAlignment obstacleAlignment = SpawnAlignment.ZAxis; //alignment van obstakels.
    public SpawnAlignment itemAlignment = SpawnAlignment.None;
    public PlacementType placementType = PlacementType.X;
    [Range(2, 12)]
    public int itemPlatformDistance = 6;
    private int curLastItemPlatform = 0;
    [Range(2, 7)]
    public int itemGridDistance = 2;
    public bool oneItemPlatform = true;
    public bool center = true; //Centers the object if possible (When the object next to it is non existent for example)
    public bool placeNextToItem = true;
    public SpawnType spawnType;
    public SpawnObjectTypeChance[] spawnObjectTypeChances;

    // Use this for initialization
    void Start()
    {
        Resize(5f, 8f, 1f, 1f, 2f, 3f);
        SpawnedObjectType[,] test = GenerateSpawnObjectTypesArray();
        Debug.Log("testeroni");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Returns an array with the grids and the item to spawn on there.
    /// </summary>
    /// <returns></returns>
    public SpawnObject[,] GenerateSpawnObjects()
    {
        return null;
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
            case SpawnType.LineZ:
                return GenerateSpawnObjectTypesArrayLineZ();
            case SpawnType.Random:
                return GenerateSpawnObjectTypesArrayRandom();
            case SpawnType.RandomLineX:
                return GenerateSpawnObjectTypesArrayRandomLineX();
            case SpawnType.RandomLineZ:
                return GenerateSpawnObjectTypesArrayRandomLineZ();
            case SpawnType.Sectioned:
                return GenerateSpawnObjectTypesArraySectioned();
            case SpawnType.Single:
                return GenerateSpawnObjectTypesArraySingle();
        }

        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array full of one object.
    /// There is offcourse a limit on obstacles.
    /// </summary>
    /// <returns>Gives back a SpawnedObjectType multidimensional array full of one object.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayPlatform()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[platformObjects.GetLength(0), platformObjects.GetLength(1)];

        SpawnedObjectType sot = GetRandomSpawnObjectType();
        if (curLastItemPlatform <= itemPlatformDistance)
        {
            while (sot == SpawnedObjectType.Item) sot = GetRandomSpawnObjectType();
        }

        switch (sot)
        {
            case SpawnedObjectType.Item:
                PlaceOneSpawnedObjectTypeRandomly(ref spawns, sot); //nog voor meerdere items.
                break;
            case SpawnedObjectType.Coin:
                FillSpawnedObjectTypeArray(ref spawns, sot);
                break;
            case SpawnedObjectType.None:
                FillSpawnedObjectTypeArray(ref spawns, sot);
                break;
            case SpawnedObjectType.Obstacle:
                PlaceObstaclesRandomly(ref spawns, SpawnedObjectType.Obstacle, minObstacleDistance, maxObstaclesPlatform, obstacleAlignment, placementType);
                break;
        }

        return spawns;
    }

    /// <summary>
    /// Places a spawned object type on the given axises in the given array.
    /// </summary>
    /// <param name="spawns">The array in which you want to place the spawnedObjectTypes.</param>
    /// <param name="sot">The SpawnedObjectType you want to place.</param>
    /// <param name="x">The x-axis you want to place it on.</param>
    /// <param name="z">The z-axis you want to place it on.</param>
    private void PlaceSpawnedObjectTypeInArray(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int x, int z)
    {
#if UNITY_EDITOR
        if (x > spawns.GetLength(0) || z > spawns.GetLength(1)) Debug.Log("Out of bound exception for: " + sot + " on the following axisses x:" + x + " z:" + z);
#endif
        spawns[x, z] = sot;
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
            for (int z = 0; z < spawns.GetLength(1); z++)
            {
                PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, z);
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
        int xPos = (center && (spawns.GetLength(0) % 2 == 1)) ? (spawns.GetLength(0) + (spawns.GetLength(0) % 2)) / 2 : UnityEngine.Random.Range(0, spawns.GetLength(0)); //Gets the center grid if center is true else it gets a random value on the x axis of the grid
        int zPos = UnityEngine.Random.Range(0, spawns.GetLength(1));
        for (int z = 0; z < spawns.GetLength(1); z++)
        {
            for (int x = 0; x < spawns.GetLength(0); x++)
            {
                if (x == xPos && z == zPos)
                {
                    PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, z);
                }
                else
                {
                    PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, z);
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
    private void PlaceObstaclesRandomly(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maxObjects, SpawnAlignment alignment = SpawnAlignment.None, PlacementType placementType = PlacementType.X)
    {
        switch (placementType)
        {
            case PlacementType.X:
                PlaceObjectsRandomlyX(ref spawns, sot, distance, maxObjects, alignment);
                break;
            case PlacementType.Z:
                PlaceObjectsRandomlyZ(ref spawns, sot, distance, maxObjects, alignment);
                break;
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
    private void PlaceObjectsRandomlyX(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maxObjects, SpawnAlignment alignment = SpawnAlignment.None)
    {
        int xPos = GetXPos(alignment, spawns.GetLength(0));
        int zPos = GetZPos(alignment, spawns.GetLength(1));

        int objectCount = 0;
        bool canSpawn = true;

        for (int z = 0; z < spawns.GetLength(1); z++)
        {
            if (!canSpawn) return;
            for (int x = 0; x < spawns.GetLength(0); x++)
            {
                if (!canSpawn) return;
                switch (alignment)
                {
                    case SpawnAlignment.None:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                        {
                            zPos = GetZPos(alignment, spawns.GetLength(1), z + distance + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om hier te gebruiken.
                            xPos = GetRandomValue(0, spawns.GetLength(0), x, distance + 1); //GetXPos(alignment, spawns.GetLength(1), x + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om te gebruiken.
                        }
                        if (zPos == -1 || xPos == -1) canSpawn = false;
                        break;
                    case SpawnAlignment.XAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                            zPos = GetZPos(alignment, spawns.GetLength(1), z + distance + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om hier te gebruiken.
                        if (zPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.XAxisFill:
                        PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, xPos, z);
                        break;
                    case SpawnAlignment.ZAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                            xPos = GetRandomValue(0, spawns.GetLength(0), x, distance + 1); //GetXPos(alignment, spawns.GetLength(1), x + 1); //Je kan niks lager krijgen dan de huidige z dus de GetRandomValue methode is eigenlijk overbodig om te gebruiken.
                        if (xPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.ZAxisFill:
                        PlaceOnRightAlignmentZ(ref spawns, ref objectCount, sot, x, z, zPos);
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
    /// <param name="maxObjects">The maximum amount of objects you want to spawn.</param>
    /// <param name="alignment"></param>
    private void PlaceObjectsRandomlyZ(ref SpawnedObjectType[,] spawns, SpawnedObjectType sot, int distance, int maxObjects, SpawnAlignment alignment = SpawnAlignment.None)
    {
        int xPos = GetXPos(alignment, spawns.GetLength(0));
        int zPos = GetZPos(alignment, spawns.GetLength(1));

        int objectCount = 0;
        bool canSpawn = true;

        for (int x = 0; x < spawns.GetLength(0); x++)
        {
            if (!canSpawn) return;
            for (int z = 0; z < spawns.GetLength(1); z++)
            {
                if (!canSpawn) return;
                switch (alignment)
                {
                    case SpawnAlignment.None:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                        {
                            zPos = GetRandomValue(0, spawns.GetLength(0), z, distance + 1); //verschil met de andere method zit hier.
                            xPos = GetXPos(alignment, spawns.GetLength(0), x + distance + 1); //verschil met de andere method zit hier.
                        }
                        if (zPos == -1 || xPos == -1) canSpawn = false;
                        break;
                    case SpawnAlignment.XAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                            zPos = GetRandomValue(0, spawns.GetLength(0), z, distance + 1); //verschil met de andere method zit hier.
                        if (zPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.XAxisFill:
                        PlaceOnRightAlignmentX(ref spawns, ref objectCount, sot, x, xPos, z);
                        break;
                    case SpawnAlignment.ZAxis:
                        if (objectCount >= maxObjects) return;
                        if (PlaceOnRightAlignment(ref spawns, ref objectCount, sot, x, xPos, z, zPos))
                            xPos = GetXPos(alignment, spawns.GetLength(0), x + distance + 1); //verschil met de andere method zit hier.
                        if (xPos == -1)
                            canSpawn = false;
                        break;
                    case SpawnAlignment.ZAxisFill:
                        PlaceOnRightAlignmentZ(ref spawns, ref objectCount, sot, x, z, zPos);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Gets a random value that is between the min and max and not between the numbers of currentValue + distance and currentValue - Distance.
    /// </summary>
    /// <param name="min">The minimum number you want.</param>
    /// <param name="max">The maximum number you want.</param>
    /// <param name="currentValue">The value you want dont want.</param>
    /// <param name="distance">The distance around the currentValue you dont want.</param>
    /// <returns>'Returns a value between min and curMin based on the current value and distance or curMax(also based on distance and currentValue) and max if it doesn't exceed the min for curMin or the max for curMax, else it will return -1.</returns>
    public int GetRandomValue(int min, int max, int currentValue, int distance)
    {
        int curMin = currentValue - distance;
        int curMax = currentValue + distance;

        bool minPossible = (curMin >= min) ? true : false;
        bool maxPossible = (curMax <= max) ? true : false;

        if (minPossible && maxPossible)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) // 0 for min 1 for max
            {
                return UnityEngine.Random.Range(curMax, max);
            }
            else
            {
                return UnityEngine.Random.Range(min, curMin);
            }
        }
        else if (minPossible)
        {
            return UnityEngine.Random.Range(min, curMin);
        }
        else if (maxPossible)
        {
            return UnityEngine.Random.Range(curMax, max);
        }
        else
        {
            return -1;
        }

    }

    /// <summary>
    /// Gets a random value that is between the min and max and not between the numbers of currentValue + distance and currentValue - Distance.
    /// </summary>
    /// <param name="min">The minimum number you want.</param>
    /// <param name="max">The maximum number you want.</param>
    /// <param name="currentValue">The value you want dont want.</param>
    /// <param name="distanceMin">The distance for the min form currentValue for curMin.</param>
    /// <param name="distanceMax">The distance for the max from currentValue for curMax.</param>
    /// <returns>'Returns a value between min and curMin based on the current value and distance or curMax(also based on distance and currentValue) and max if it doesn't exceed the min for curMin or the max for curMax, else it will return -1.</returns>
    public int GetRandomValue(int min, int max, int currentValue, int distanceMin, int distanceMax)
    {
        int curMin = currentValue - distanceMin;
        int curMax = currentValue + distanceMax;

        bool minPossible = (curMin >= min) ? true : false;
        bool maxPossible = (curMax <= max) ? true : false;

        if (minPossible && maxPossible)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) // 0 for min 1 for max
            {
                return UnityEngine.Random.Range(curMax, max);
            }
            else
            {
                return UnityEngine.Random.Range(min, curMin);
            }
        }
        else if (minPossible)
        {
            return UnityEngine.Random.Range(min, curMin);
        }
        else if (maxPossible)
        {
            return UnityEngine.Random.Range(curMax, max);
        }
        else
        {
            return -1;
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
    private void PlaceOnRightAlignmentX(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int xPos, int z, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (x == xPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, z);
            objectCount++;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, defaultObjectType, x, z);
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
    private void PlaceOnRightAlignmentZ(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int z, int zPos, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (z == zPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, z);
            objectCount++;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, z);
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
    private bool PlaceOnRightAlignment(ref SpawnedObjectType[,] spawns, ref int objectCount, SpawnedObjectType sot, int x, int xPos, int z, int zPos, SpawnedObjectType defaultObjectType = SpawnedObjectType.None)
    {
        if (x == xPos && z == zPos)
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, sot, x, z);
            objectCount++;
            return true;
        }
        else
        {
            PlaceSpawnedObjectTypeInArray(ref spawns, SpawnedObjectType.None, x, z);
            return false;
        }
    }



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
            case SpawnAlignment.ZAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.ZAxisFill:
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
    public int GetZPos(SpawnAlignment alignment, int max, int min = 0)
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
            case SpawnAlignment.ZAxis:
                return UnityEngine.Random.Range(min, max);
            case SpawnAlignment.ZAxisFill:
                return UnityEngine.Random.Range(min, max);
        }
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each x-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each x-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayLineX()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each z-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one single object or multiple objects of the same type on each z-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayLineZ()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array based on sections.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array based on sections.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandom()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with randomObjects sorted line based on the x-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with randomObjects sorted line based on the x-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandomLineX()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with randomObjects sorted line based on the z-axis.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with randomObjects sorted line based on the z-axis.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArrayRandomLineZ()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array with one object that isn't none.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array with one object that isn't none.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArraySingle()
    {
        return null;
    }

    /// <summary>
    /// Gives back a SpawnedObjectType multidimensional array based on sections.
    /// </summary>
    /// <returns>A SpawnedObjectType multidimensional array based on sections.</returns>
    private SpawnedObjectType[,] GenerateSpawnObjectTypesArraySectioned()
    {
        return null;
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
        platformObjects = new SpawnableObject[GetGridsX(), GetGridsZ()];
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
    private int GetGridsZ()
    {
        int grids = 0;
        float zSize = transform.localScale.z;
        while (zSize > gridSize.z)
        {
            zSize -= gridSize.z;
            grids++;
        }
        return grids;
    }

}
