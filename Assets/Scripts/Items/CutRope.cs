using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutRope : MonoBehaviour, ISlashable
{
    bool m_cut = false;
    public Rigidbody HeldObject;

    /// <summary>
    /// Only slash once.
    /// </summary>
    /// <returns></returns>
    public bool CanBeSlashed()
    {
        return !m_cut;
    }

    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    /// <summary>
    /// Cut the rope.
    /// </summary>
    public void OnSlashed(Vector3 direction)
    {
        HeldObject.isKinematic = false;
    }
}
