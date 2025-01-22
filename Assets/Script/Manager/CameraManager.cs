using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private float camOffset = 1;
    [SerializeField] private Transform[] camPointArray = new Transform[4];

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetupCamera(float xMin, float xMax, float yMin, float yMax)
    {
        camPointArray[0].localPosition = new Vector3((xMin + xMax) / 2, yMax + camOffset);
        camPointArray[1].localPosition = new Vector3(xMin -camOffset, 0);
        camPointArray[2].localPosition = new Vector3(xMax + camOffset, 0);
        camPointArray[3].localPosition = new Vector3(0, yMin - camOffset);
    }
}
