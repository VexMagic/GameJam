using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpData : MonoBehaviour
{
    [SerializeField] private float JumpHeight = 1;
    [SerializeField] private float JumpPush = 1;

    public float jumpHeight => JumpHeight;
    public float jumpPush => JumpPush;
}
