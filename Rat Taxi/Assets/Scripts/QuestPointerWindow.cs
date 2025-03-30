using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Reflection;

public class QuestPointerWindow : MonoBehaviour
{
    public static QuestPointerWindow instance;

    [SerializeField] private Camera uiCamera;
    [SerializeField] private float boarderSize;
    [SerializeField] private Vector3 onScreenOffset;
    [SerializeField] private float maxDistance;

    [SerializeField] private GameObject pointerObject;

    private List<Pointer> pointers = new List<Pointer>();

    private void Awake()
    {
        instance = this;

        RemovePointers();
    }

    private void Update()
    {
        foreach (var pointer in pointers)
        {
            SetPointerPos(pointer);
        }
    }

    private void SetPointerPos(Pointer pointer)
    {
        Vector3 targetPosScreenPoint = Camera.main.WorldToScreenPoint(pointer.target);
        bool isOffScreen = targetPosScreenPoint.x <= boarderSize || targetPosScreenPoint.x >= Screen.width - boarderSize
            || targetPosScreenPoint.y <= boarderSize || targetPosScreenPoint.y >= Screen.height - boarderSize;

        if (isOffScreen)
        {
            pointer.animator.SetBool("Arrow", true);
            RotatePointerTowardsTarget(pointer);

            Vector3 cappedTargetScreenPosition = new Vector3(Mathf.Clamp(targetPosScreenPoint.x, boarderSize, Screen.width - boarderSize),
                Mathf.Clamp(targetPosScreenPoint.y, boarderSize, Screen.height - boarderSize));

            Vector3 pointerWorldPos = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointer.pos.position = pointerWorldPos;
            pointer.pos.localPosition = new Vector3(pointer.pos.localPosition.x, pointer.pos.localPosition.y, 0f);

            float dist = Vector3.Distance(pointer.target, CameraManager.instance.transform.position);
            float percentage = Mathf.Clamp01(dist / maxDistance);
            pointer.pos.localScale = new Vector3(1.5f - percentage, 1.5f - percentage);
        }
        else
        {
            pointer.animator.SetBool("Arrow", false);
            pointer.pos.localEulerAngles = Vector3.zero;

            Vector3 pointerWorldPos = uiCamera.ScreenToWorldPoint(targetPosScreenPoint);
            pointer.pos.position = pointerWorldPos + onScreenOffset;
            pointer.pos.localPosition = new Vector3(pointer.pos.localPosition.x, pointer.pos.localPosition.y, 0f);

            pointer.pos.localScale = new Vector3(1.5f, 1.5f);
        }
    }

    private void RotatePointerTowardsTarget(Pointer pointer)
    {
        Vector3 toPosition = pointer.target;
        Vector3 fromPosition = CameraManager.instance.transform.position;
        fromPosition.z = 0;
        Vector3 dir = (toPosition - fromPosition).normalized;

        float angle = UtilsClass.GetAngleFromVectorFloat(dir);
        pointer.pos.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void RemovePointers()
    {
        foreach (var item in pointers)
        {
            Destroy(item.gameObject);
        }
        pointers.Clear();
    }

    public void AddPointer(Vector3 position) 
    {
        GameObject tempPointer = Instantiate(pointerObject, transform);
        Pointer pointerData = tempPointer.GetComponent<Pointer>();
        pointerData.SetTarget(position);

        pointers.Add(pointerData);
    }
}
