using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public ThirdPersonCharacterController playerController;
    public CameraFollow cameraFollow;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
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
    }
}
