using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TRandom
{
    /// <summary>
    /// Gets a random value that is between the min and max and not between the numbers of currentValue + distance and currentValue - Distance.
    /// </summary>
    /// <param name="min">The minimum number you want.</param>
    /// <param name="max">The maximum number you want.</param>
    /// <param name="currentValue">The value you want dont want.</param>
    /// <param name="distance">The distance around the currentValue you dont want.</param>
    /// <returns>'Returns a value between min and curMin based on the current value and distance or curMax(also based on distance and currentValue) and max if it doesn't exceed the min for curMin or the max for curMax, else it will return -1.</returns>
    public static int GetRandomValue(int min, int max, int currentValue, int distance)
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
    public static int GetRandomValue(int min, int max, int currentValue, int distanceMin, int distanceMax)
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
    /// Gets a randomValue between the min and the max which is not in the excluded set.
    /// </summary>
    /// <param name="min">The minimum value of the array you want.</param>
    /// <param name="max">The maximum value of the array you want.</param>
    /// <param name="excluded">The excluded numbers.</param>
    /// <returns>A randomValue between the min and the max which is not in the excluded set.</returns>
    public static int GetRandomValue(int min, int max, HashSet<int> excluded)
    {
        int[] numberArray = new int[(max - excluded.Count)];
        int curExcluded = 0;
        for (int i = 0; i < max; i++)
        {
            if (excluded.Contains(i))
            {
                curExcluded++;
            }
            else
            {
                numberArray[i - curExcluded] = i;
            }
        }
        return numberArray[UnityEngine.Random.Range(0, numberArray.Length)];
    }
}
