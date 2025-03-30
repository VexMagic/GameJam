using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private Camera cameraValues;
    [SerializeField] private GameObject followObject;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 sizeRange;

    private float cameraZ;

    private void Awake()
    {
        instance = this;
        cameraZ = transform.position.z;
        transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, cameraZ);
    }

    private void Update()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, followObject.transform.position.x, Time.deltaTime * moveSpeed),
            Mathf.Lerp(transform.position.y, followObject.transform.position.y, Time.deltaTime * moveSpeed), cameraZ);

        cameraValues.orthographicSize = Mathf.Lerp(sizeRange.x, sizeRange.y, CarController.instance.GetSpeedPercentage());
    }
}
