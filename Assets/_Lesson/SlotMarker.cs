﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType { LeftWeapon, RightWeapon, LeftHand, RightHand, LeftBack, RightBack, LeftShotPos, RightShotPos }

public class SlotMarker : MonoBehaviour
{
    public SlotType slotType;
}