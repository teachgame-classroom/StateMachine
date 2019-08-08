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
        if (this.questType == QuestType.FindItem)
        {
            if (this.targetProgress >= this.targetCount)
            {
                return true;
            }
        }

        return false;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public QuestList questList;

    private List<QuestProgress> questProgresses = new List<QuestProgress>();

    public delegate void QuestsDelegate(Quest[] quests);
    public event QuestsDelegate SendQuestsEvent;

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
            UIManager.instance.OnQuestStatusChange(idx, "进行中");
        }
        else
        {
            if(accpetedProgress != null)
            {
                if(accpetedProgress.questStatus == QuestStatus.FinishedButNotAwarded)
                {
                    accpetedProgress.questStatus = QuestStatus.Completed;
                    UIManager.instance.OnQuestStatusChange(idx, "完成");
                }
            }
        }
    }

    public void OnInventoryChange(InventorySlotType slotType, int gridIdx, int itemId, int itemCount, Sprite sprite, RarityType rarityType)
    {
        Debug.Log("任务系统收到背包改变事件");

        for(int i = 0; i < questProgresses.Count; i++)
        {
            QuestProgress progress = questProgresses[i];

            if (progress.questType == QuestType.FindItem)
            {
                if(progress.targetItemID == itemId)
                {
                    progress.targetProgress = itemCount;
                    Debug.Log("任务目标物品数量变为" + progress.targetProgress);
                }

                if(progress.questStatus == QuestStatus.Accepted && progress.CheckProgress())
                {
                    Debug.Log("任务物品收集完成");
                    progress.questStatus = QuestStatus.FinishedButNotAwarded;
                    UIManager.instance.OnQuestStatusChange(progress.questId, "领赏");
                }
            }
        }
    }
}

