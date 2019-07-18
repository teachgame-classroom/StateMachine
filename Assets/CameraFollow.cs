﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { Freelook, LookAt, LockOn, ProfileLeft, ProfileRight, FixedAngle }

public class CameraFollow : MonoBehaviour
{
    public Transform[] pivotPosTrans;
 
    public Transform followTarget;
    public Transform lookAtTarget;

    public CameraState cameraState;
    private CameraState cameraStateLastFrame;

    public float smooth = 5;

    public float mouseSensitivity_X = 10;
    public float mouseSensitivity_Y = 2;

    private Transform pivot;
    private Transform camTrans;
    private Camera cam;

    private Vector3 targetRigDirection;

    private Vector3 additionalPivotOffset;

    private Vector3 freelookPivotDirection;
    private float freelookPivotDistance;
    private float additionalFreelookPivotDistance;

    private float fov = 60;
    private float yaw_root;
    private float yaw;
    private float pitch;

    private Quaternion targetRotYaw_root;
    private Quaternion targetRotYaw;
    private Quaternion targetRotPitch;

    // Start is called before the first frame update
    void Start()
    {
        pivot = transform.Find("Pivot");
        cam = pivot.GetComponentInChildren<Camera>();
        camTrans = cam.transform;

        pivotPosTrans = new Transform[GetComponentsInChildren<ObjectMarker>().Length];

        foreach (ObjectMarker marker in GetComponentsInChildren<ObjectMarker>())
        {
            int id = (int)marker.slotType;
            pivotPosTrans[id] = marker.transform;
        }

        freelookPivotDirection = pivotPosTrans[(int)MarkerType.CameraPivotPos_Normal].localPosition;
        freelookPivotDistance = freelookPivotDirection.magnitude;
        freelookPivotDirection.Normalize();

        SetFollowTarget(followTarget);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraState != cameraStateLastFrame)
        {
            SetCameraState(cameraState);
        }

        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if(cameraState != CameraState.Freelook)
            {
                additionalPivotOffset += Vector3.forward * 0.5f;
            }
            else
            {
                additionalFreelookPivotDistance += 0.5f;
            }

        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (cameraState != CameraState.Freelook)
            {
                additionalPivotOffset -= Vector3.forward * 0.5f;
            }
            else
            {
                additionalFreelookPivotDistance -= 0.5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            additionalPivotOffset += Vector3.right * 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            additionalPivotOffset -= Vector3.right * 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            additionalPivotOffset = Vector3.zero;
        }

        float smoothFactor = 1;

        if (cameraState == CameraState.Freelook)
        {
            MoveCameraByMouse();
            camTrans.localRotation = Quaternion.Lerp(camTrans.localRotation, targetRotPitch, smooth * Time.deltaTime);
            pivot.localRotation = Quaternion.Lerp(pivot.localRotation, targetRotYaw, smooth * Time.deltaTime);

            Vector3 freelookPivotTargetPos = transform.position + transform.TransformDirection(freelookPivotDirection) * (freelookPivotDistance + additionalFreelookPivotDistance);

            pivot.position = Vector3.Lerp(pivot.position, freelookPivotTargetPos, 5 * Time.deltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotYaw_root, smooth * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, smooth * Time.deltaTime);

        }
        else if (cameraState == CameraState.LookAt)
        {
            LookAt(lookAtTarget);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotYaw_root, smooth * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, smooth * Time.deltaTime);
            //camTrans.rotation = Quaternion.Lerp(camTrans.rotation, targetRotPitch, smooth * Time.deltaTime);
        }
        else if (cameraState == CameraState.LockOn)
        {
            LockOn(lookAtTarget);
            smoothFactor = 2;
        }
        else if (cameraState == CameraState.ProfileRight || cameraState == CameraState.ProfileLeft || cameraState == CameraState.FixedAngle)
        {
            FixedAngle();
        }

        transform.position = Vector3.Lerp(transform.position, followTarget.position, Mathf.Clamp01(smooth * smoothFactor * Time.deltaTime));
        cameraStateLastFrame = cameraState;
    }

    public void SetCameraState(CameraState newState)
    {
        switch (newState)
        {
            case CameraState.Freelook:
                break;
            case CameraState.LookAt:
                break;
            case CameraState.LockOn:
                break;
            case CameraState.ProfileLeft:
                targetRigDirection = followTarget.right;
                break;
            case CameraState.ProfileRight:
                targetRigDirection = -followTarget.right;
                break;
            case CameraState.FixedAngle:
                break;
            default:
                break;
        }

        cameraState = newState;
    }

    public void SetFixedAngle(Vector3 rigDirection)
    {
        targetRigDirection = rigDirection;
        SetCameraState(CameraState.FixedAngle);
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;

        targetRotYaw_root = Quaternion.LookRotation(followTarget.forward);

        yaw_root = targetRotYaw_root.eulerAngles.y;

        ICameraFollowable followable = followTarget.GetComponent<ICameraFollowable>();

        if(followable != null)
        {
            followable.cameraFollow = this;
        }
    }

    public void SetLockOnTarget(Transform target)
    {
        if(target != null)
        {
            lookAtTarget = target;
            cameraState = CameraState.LockOn;
        }
        else
        {
            cameraState = CameraState.Freelook;
        }
    }

    public void MoveCameraByMouse()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float deltaYaw = mouseX * mouseSensitivity_X;
        float deltaPitch = -mouseY * mouseSensitivity_Y;

        SetRootYawAngle(yaw_root + deltaYaw);
        SetPitchAngle(pitch + deltaPitch);

        fov = 60;
    }

    public void StartLookAt(float duration = 2)
    {
        CameraState lastCameraState = cameraState;
        cameraState = CameraState.LookAt;
        //lookAt = true;
        Scheduler.instance.Schedule(duration, false, () => { cameraState = lastCameraState; });
    }

    public void LookAt(Transform target)
    {
        Vector3 lookDirection = target.position - camTrans.position;
        Vector3 lookDirection_Yaw = Vector3.Scale(new Vector3(1, 0, 1), lookDirection);
        Vector3 lookDirection_Pitch = Vector3.Scale(new Vector3(0, 1, 1), lookDirection);

        Quaternion lookRotationYaw = Quaternion.LookRotation(lookDirection_Yaw);
        Quaternion lookRotationPitch = Quaternion.LookRotation(lookDirection_Pitch);

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        targetRotYaw_root = lookRotationYaw;
        targetRotPitch = lookRotation;

        SetYawAngle(0);

        fov = 30;
    }

    public void LockOn(Transform target)
    {
        if(target != null)
        {
            Vector3 direction = target.position - followTarget.position;
            SetRigDirection(direction);
            SetPivotOffset(pivotPosTrans[(int)MarkerType.CameraPivotPos_LockOn].position);

            Vector3 lookDirection = target.position - transform.position;

            SetCameraLookDirection(lookDirection);
        }
    }

    public void FixedAngle()
    {
        SetRigDirection(targetRigDirection);
        SetPivotOffset(pivotPosTrans[(int)MarkerType.CameraPivotPos_FixedAngle].position);
        SetCameraLookDirection(pivot.forward);
    }


    public void SetCameraLookDirection(Vector3 direction)
    {
        direction.Normalize();
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        camTrans.rotation = lookRotation;
    }

    public void SetRigDirection(Vector3 direction)
    {
        direction.Normalize();
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 540 * Time.deltaTime);
    }

    public void SetPivotOffset(Vector3 offset)
    {
        offset = offset + pivot.TransformDirection(additionalPivotOffset);
        pivot.position = Vector3.Lerp(pivot.position, offset, 5 * Time.deltaTime);
    }

    public void SetTargetAngle(float yaw, float pitch)
    {
        SetYawAngle(yaw);
        SetPitchAngle(pitch);
    }

    public void SetYawAngle(float yaw)
    {
        this.yaw = yaw;
        targetRotYaw = Quaternion.Euler(0, this.yaw, 0);
    }

    public void SetPitchAngle(float pitch)
    {
        this.pitch = pitch;
        targetRotPitch = Quaternion.Euler(this.pitch, 0, 0);
    }

    public void SetRootYawAngle(float yaw)
    {
        this.yaw_root = yaw;
        targetRotYaw_root = Quaternion.Euler(0, this.yaw_root, 0);
    }
}
