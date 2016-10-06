using UnityEngine;
using System.Collections;

public interface IResource {
    ResourceType GetResourceType();
    void PickUp();
}
