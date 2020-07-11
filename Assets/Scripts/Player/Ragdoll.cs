using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public List<Collider> Colliders;
    public List<Rigidbody> Rigidbodies;

    public Animator Animator;

    /// <summary>
    /// Set the rigidbody enabled or disabled.
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        // Toggle the animator.
        Animator.enabled = !enabled;

        // Toggle the rigidbodies.
        foreach (var r in Rigidbodies)
        {
            r.isKinematic = !enabled;
        }

        // Toggle the colliders.
        foreach (var c in Colliders)
        {
            c.enabled = enabled;
        }
    }
}
