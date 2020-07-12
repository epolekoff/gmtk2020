using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenButton : Item
{
    public Door Door;
    public bool Open;

    /// <summary>
    /// Push the guy off the bridge.
    /// </summary>
    protected override void PushButton(Hand hand)
    {
        Door.SetOpen(Open);

        // Only press closed once.
        if(!Open)
        {
            PickUpRange.enabled = false;
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPressSound);
    }
}
