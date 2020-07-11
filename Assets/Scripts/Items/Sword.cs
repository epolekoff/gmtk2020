using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody.isKinematic = true;
    }
}
