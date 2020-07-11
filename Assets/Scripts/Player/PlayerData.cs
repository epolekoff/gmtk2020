using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "Data/PlayerCharacter")]
public class PlayerData : ScriptableObject
{
    public float JumpVelocity = 5;
    public float DelayBeforeJump = 1f;

    public float MoveSpeed = 1f;
    public float AirMoveSpeed = 1f;
    public float Friction = 1f;
    public float MaxVelocity = 30f;
}
