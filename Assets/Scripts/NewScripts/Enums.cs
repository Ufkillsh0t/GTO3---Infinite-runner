using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum SpawnedObjectType
{
    None,
    Coin,
    Item,
    Obstacle
}

public enum SpawnObject
{
    None,
    SmallCoin,
    MediumCoin,
    BigCoin,
    Magnet,
    SpeedBoost,
    SlowDown,
    PlatformIncrease,
    PlatformDecrease,
    Spike,
    Box,
    Mud,
    HoveringBox
}

public enum SpawnType
{
    Platform,
    LineX,
    LineY,
    Random,
    RandomLineX,
    RandomLineY,
    Single,
    Sectioned
}

public enum SpawnAlignment
{
    YAxis,
    XAxis,
    YAxisFill, //Ignores the maximum and fills the entire row.
    XAxisFill, //Ignores the maximum and fills the entire row.
    None
}

public enum PlacementType
{
    X,
    Y
}

public enum MovementAxis
{
    None,
    X,
    Y,
    Z
}

public enum NextFloor
{
    Upper,
    Same,
    Lower
}