using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    /// <summary>
    /// Trigger death.
    /// </summary>
    void OnTriggerEnter(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            pc.Die();
        }
    }
}
