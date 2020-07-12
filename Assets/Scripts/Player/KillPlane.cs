using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        PlayerCharacter pc = col.GetComponentInParent<PlayerCharacter>();
        if (pc != null)
        {
            pc.Die();
        }

    }
}
