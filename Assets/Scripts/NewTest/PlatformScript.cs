using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

    public enum SpawnType
    {
        LineX,
        LineZ,
        Platform,
        Random,
        RandomLineX,
        RandomLineZ,
        Single,
        Sectioned
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
    public bool oneItemPlatform = true;
    public bool center = true; //Centers the object if possible (When the object next to it is non existent for example)
    public bool placeNextToItem = true;
    public SpawnType spawnType;
    public SpawnObjectTypeChance[] spawnObjectTypeChances;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Returns an array with the grids and the item to spawn on there.
    /// </summary>
    /// <returns></returns>
    public SpawnObject[,] GenerateSpawnObjects()
    {
        return null;
    }

    public SpawnedObjectType[,] GenerateSpawnObjectTypesArray()
    {
        SpawnedObjectType[,] spawns = new SpawnedObjectType[GetGridsX(), GetGridsZ()];  //Misschien vervangen naar de dimensie van de huidige platformObjects.

        return null;
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
        foreach(SpawnObjectTypeChance sotc in spawnObjectTypeChances)
        {
            if(sotc.spawnedObjectType == objectType)
            {
                float spawnFloat = UnityEngine.Random.Range(minSpawnObjectChance, maxSpawnObjectChance);
                foreach(SpawnChance sc in sotc.spawnChances)
                {
                    if(sc.spawnChance < spawnFloat)
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
