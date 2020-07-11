using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlashable
{
    bool CanBeSlashed();
    void OnSlashed(Vector3 direction);
    GameObject GetGameObject();
}
