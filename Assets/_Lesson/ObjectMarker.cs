using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType { CameraPivotPos_Normal, CameraPivotPos_LockOn, CameraPivotPos_FixedAngle, LookAtPos = 99 }

public class ObjectMarker : MonoBehaviour
{
    public MarkerType slotType;
}
