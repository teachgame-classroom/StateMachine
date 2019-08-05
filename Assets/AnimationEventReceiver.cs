using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public ThirdPersonCharacterController controller;

    void Start()
    {
        controller = GetComponentInParent<ThirdPersonCharacterController>();
    }

    public void AttachWeapon(int weaponPosIdx)
    {
        controller.AttachWeapon(weaponPosIdx);
    }

    public void OnAnimationEvent(string eventName)
    {
        controller.OnAnimationEvent(eventName);
    }

    public void WpnPullTrigerRight(int idx)
    {
        controller.WpnPullTrigerRight(idx);
    }

    public void WpnPullTrigerLeft(int idx)
    {
        controller.WpnPullTrigerLeft(idx);
    }

}
