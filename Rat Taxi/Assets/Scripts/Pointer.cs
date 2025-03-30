using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
    [SerializeField] private RectTransform Pos;
    [SerializeField] private Animator Animator;

    private Vector3 Target;

    public RectTransform pos => Pos;
    public Animator animator => Animator;
    public Vector3 target => Target;

    public void SetTarget(Vector3 target)
    {
        Target = target;
    }
}
