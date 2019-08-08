using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestList", menuName = "Quest List")]
public class QuestList : ScriptableObject
{
    public Quest[] quests;
}


