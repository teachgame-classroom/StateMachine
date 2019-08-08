using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus { NotAccepted, Accepted, FinishedButNotAwarded, Completed }
public enum QuestType { FindItem, KillEnemy, GoToSomewhere}

[System.Serializable]
public class Quest
{
    public string questName;
    public int questId;
    public string questDescription;
    public QuestType questType;
    public int questTargetItemID;
    public int questTargetDestinationIdx;
    public int questTargetCount;

    public static string GetStatusStringCn(QuestStatus status)
    {
        string result;

        switch (status)
        {
            case QuestStatus.NotAccepted:
                result = "未接受";
                break;
            case QuestStatus.Accepted:
                result = "进行中";
                break;
            case QuestStatus.FinishedButNotAwarded:
                result = "领赏";
                break;
            case QuestStatus.Completed:
                result = "完成";
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        return result;
    }

    public static Color GetColorByStatus(QuestStatus status)
    {
        Color result;

        switch (status)
        {
            case QuestStatus.NotAccepted:
                result = Color.red;
                break;
            case QuestStatus.Accepted:
                result = Color.green;
                break;
            case QuestStatus.FinishedButNotAwarded:
                result = Color.yellow;
                break;
            case QuestStatus.Completed:
                result = Color.gray;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        return result;

    }
}
