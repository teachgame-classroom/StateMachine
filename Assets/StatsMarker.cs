using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsType { ATK, DEF, DEX, HP, MANA, FURY, RECOVERY_HP, RECOVERY_MANA, RECOVERY_FURY }

public class StatsMarker : MonoBehaviour
{
    public StatsType statsType;
}
