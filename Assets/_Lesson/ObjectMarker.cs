using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType { CameraPivotPos_Normal, CameraPivotPos_LockOn, CameraPivotPos_FixedAngle }

public class ObjectMarker : MonoBehaviour
{
    public MarkerType slotType;
}
