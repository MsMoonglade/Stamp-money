using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineVirtualCameraSwitcher : MonoBehaviour
{
    public static CinemachineVirtualCameraSwitcher instance;

    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera editCamera;

    private void Awake()
    {
        instance = this;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.Priority = 11;
        editCamera.Priority = 0;
    }

    public void SwitchToEditCamera()
    {
        editCamera.Priority = 11;
        playerCamera.Priority = 0;
    }
}