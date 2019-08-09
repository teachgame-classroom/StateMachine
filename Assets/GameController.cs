using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private QuestManager questManager;

    public ThirdPersonCharacterController playerController;
    public CameraFollow cameraFollow;

    public delegate void EnemyNotifyDelegate(ThirdPersonCharacterController controller);
    public event EnemyNotifyDelegate EnemyDieEvent;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        questManager = GetComponent<QuestManager>();

        questManager.SendQuestsEvent += UIManager.instance.OnQuestsEvent;
        UIManager.instance.QuestButtonClickEvent += questManager.OnQuestButtonClicked;
    }

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<ThirdPersonCharacterController>();
        cameraFollow = GameObject.Find("CameraRig").GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            playerController.inventory.Refresh();
            UIManager.instance.ToggleCharacterPanel();
            cameraFollow.freezeCamera = !cameraFollow.freezeCamera;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            UIManager.instance.ToggleQuestPanel();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            UIManager.instance.ToggleShopPanel();
        }
    }

    public void OnMapEvent(string eventName, params object[] para)
    {
        Debug.Log("收到地图事件：" + eventName + "," + para);

        if(eventName == "PlatformPos")
        {
            switch((int)para[0])
            {
                case 0:
                    cameraFollow.SetFixedAngle(Vector3.right);
                    playerController.SetMoveInputAxis(MoveInputAxis.Horizontal);
                    playerController.transform.position = new Vector3(((Vector3)para[1]).x, playerController.transform.position.y, playerController.transform.position.z);
                    playerController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                    break;
                case 1:
                    cameraFollow.SetFixedAngle(Vector3.forward);
                    playerController.SetMoveInputAxis(MoveInputAxis.Horizontal);
                    playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, ((Vector3)para[1]).z);
                    playerController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    break;
                default:
                    break;
            }
        }

        QuestManager.instance.ReachDestination((int)para[0]);
    }

    public void EnemyDie(ThirdPersonCharacterController controller)
    {
        Debug.Log("类型为" + controller.characterType + "的敌人" + controller.gameObject.name + "挂了");
        if(EnemyDieEvent != null)
        {
            EnemyDieEvent(controller);
        }
    }
}
