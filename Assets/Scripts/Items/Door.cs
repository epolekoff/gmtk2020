using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator Animator;

    public bool Open;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        SetOpen(Open);
    }

    /// <summary>
    /// Open/Close
    /// </summary>
    public void SetOpen(bool open)
    {
        Open = open;

        Animator.Play(open ? "DoorOpen" : "DoorClose");
    }
}
