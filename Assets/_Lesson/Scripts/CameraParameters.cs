using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CameraParameters.asset", menuName = "Camera State")]
public class CameraParameters : ScriptableObject
{
    public bool mouseLook;
    public bool lookAt;
    public bool rotateRigTowardsTarget;
    public Vector3 fixedRigDirection;
    public bool localSpaceRigDirection;
    public Vector3 fixedLookDirection;
    public bool localSpaceLookDirection;
    public Vector3 pivotOffset;
    public float smooth = 1;
    public float fov = 60;
}
