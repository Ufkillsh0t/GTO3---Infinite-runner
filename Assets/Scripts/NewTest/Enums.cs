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