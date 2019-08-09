using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestProgress
{
    public int questId;
    public QuestType questType;
    public QuestStatus questStatus;
    public int targetItemID;
    public int targetCount;
    public int targetDestinationIdx;
    public int targetProgress;
    public bool isDone;

    public QuestProgress(int questId, QuestType questType, int targetItemID, int targetDestinationIdx, int targetCount)
    {
        this.questId = questId;
        this.questType = questType;
        this.targetItemID = targetItemID;
        this.targetDestinationIdx = targetDestinationIdx;
        this.targetCount = targetCount;
        this.targetProgress = 0;
    }

    public bool CheckProgress()
    {
        if (this.questType == QuestType.FindItem || this.questType == QuestType.KillEnemy)
        {
            if (this.targetProgress >= this.targetCount)
            {
                return true;
            }
        }

        return false;
    }

    public static string GetStatusText(QuestStatus questStatus)
    {
        switch (questStatus)
        {
            case QuestStatus.NotAccepted:
                return "未接受";
            case QuestStatus.Accepted:
                return "进行中";
            case QuestStatus.FinishedButNotAwarded:
                return "领赏";
            case QuestStatus.Completed:
                return "完成";
            default:
                return "未接受";
        }
    }

    public static Color GetStatusColor(QuestStatus questStatus)
    {
        switch (questStatus)
        {
            case QuestStatus.NotAccepted:
                return Color.red;
            case QuestStatus.Accepted:
                return Color.green;
            case QuestStatus.FinishedButNotAwarded:
                return Color.yellow;
            case QuestStatus.Completed:
                return Color.gray;
            default:
                return Color.white;
        }
    }


}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public QuestList questList;

    private List<QuestProgress> questProgresses = new List<QuestProgress>();

    public delegate void QuestsDelegate(Quest[] quests);
    public event QuestsDelegate SendQuestsEvent;

    public delegate void QuestProgressDelegate(QuestProgress questProgress);
    public event QuestProgressDelegate QuestProgressEvent;

    private Inventory inventory;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(SendQuestsEvent != null)
        {
            SendQuestsEvent(questList.quests);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuestButtonClicked(int idx)
    {
        Quest quest = questList.quests[idx];

        bool alreadyAccepted = false;

        QuestProgress accpetedProgress = null;

        foreach(QuestProgress progress in questProgresses)
        {
            if(progress.questId == idx)
            {
                alreadyAccepted = true;
                accpetedProgress = progress;
            }
        }

        if(!alreadyAccepted)
        {
            QuestProgress newProgress = new QuestProgress(idx, quest.questType, quest.questTargetItemID, quest.questTargetDestinationIdx, quest.questTargetCount);
            newProgress.questStatus = QuestStatus.Accepted;
            questProgresses.Add(newProgress);
            UIManager.instance.OnQuestStatusChange(idx, QuestProgress.GetStatusText(newProgress.questStatus), QuestProgress.GetStatusColor(newProgress.questStatus));

            if (QuestProgressEvent != null)
            {
                QuestProgressEvent(newProgress);
            }            
        }
        else
        {
            if(accpetedProgress != null)
            {
                if(accpetedProgress.questStatus == QuestStatus.FinishedButNotAwarded)
                {
                    accpetedProgress.questStatus = QuestStatus.Completed;
                    UIManager.instance.OnQuestStatusChange(idx, QuestProgress.GetStatusText(accpetedProgress.questStatus), QuestProgress.GetStatusColor(accpetedProgress.questStatus));
                }
            }
        }
    }

    public void OnInventoryChange(InventorySlotType slotType, int gridIdx, int itemId, int itemCount, Sprite sprite, RarityType rarityType)
    {
        Debug.Log("任务系统收到背包改变事件");

        ReceiveItemCount(itemId, itemCount);
    }

    public void OnEnemyDie(ThirdPersonCharacterController controller)
    {
        for (int i = 0; i < questProgresses.Count; i++)
        {
            QuestProgress progress = questProgresses[i];

            if (progress.questType == QuestType.KillEnemy)
            {
                if (progress.targetItemID == controller.characterType)
                {
                    progress.targetProgress++;
                    Debug.Log("已消灭" + progress.targetProgress + "个" + progress.targetItemID + "号敌人");
                }

                if (progress.questStatus == QuestStatus.Accepted)
                {
                    if (progress.CheckProgress())
                    {
                        Debug.Log("已消灭足够的目标敌人，ID：" + progress.targetItemID);
                        progress.questStatus = QuestStatus.FinishedButNotAwarded;
                    }

                    UIManager.instance.OnQuestStatusChange(progress.questId, QuestProgress.GetStatusText(progress.questStatus), QuestProgress.GetStatusColor(progress.questStatus));
                }
            }
        }
    }

    public void ReachDestination(int idx)
    {
        Debug.Log("到达了任务地点" + idx);

        for (int i = 0; i < questProgresses.Count; i++)
        {
            QuestProgress progress = questProgresses[i];

            if (progress.questType == QuestType.GoToSomewhere)
            {
                if (progress.targetDestinationIdx == idx)
                {
                    if (progress.questStatus == QuestStatus.Accepted)
                    {
                        progress.questStatus = QuestStatus.FinishedButNotAwarded;
                        UIManager.instance.OnQuestStatusChange(progress.questId, QuestProgress.GetStatusText(progress.questStatus), QuestProgress.GetStatusColor(progress.questStatus));
                    }
                }
            }
        }
    }

    public void ReceiveItemCount(int itemId, int itemCount)
    {
        for (int i = 0; i < questProgresses.Count; i++)
        {
            QuestProgress progress = questProgresses[i];

            if (progress.questType == QuestType.FindItem)
            {
                if (progress.targetItemID == itemId)
                {
                    progress.targetProgress = itemCount;
                    Debug.Log("任务目标物品数量变为" + progress.targetProgress);
                }

                if (progress.questStatus == QuestStatus.Accepted)
                {
                    if (progress.CheckProgress())
                    {
                        Debug.Log("任务物品收集完成");
                        progress.questStatus = QuestStatus.FinishedButNotAwarded;
                    }

                    UIManager.instance.OnQuestStatusChange(progress.questId, QuestProgress.GetStatusText(progress.questStatus), QuestProgress.GetStatusColor(progress.questStatus));
                }
            }
        }
    }
}

