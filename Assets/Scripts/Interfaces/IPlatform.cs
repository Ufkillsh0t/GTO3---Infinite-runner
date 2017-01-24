using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlatform {
    void Pickup(float range);
    PlatformScript GetNextPlatform(Vector3 position, NextFloor nf);
}
