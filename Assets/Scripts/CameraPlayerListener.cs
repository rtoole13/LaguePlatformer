using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerListener : MonoBehaviour
{
    public float closeViewOrthoSize;
    public float farViewOrthoSize;
    public float lerpTime;

    [SerializeField]
    private PlayerMovementModel playerMovementModel;

    private Cinemachine.CinemachineVirtualCamera virtualCamera;
    private bool isZooming = false;
    private bool isCloseView = true;

    private float lerpStartTime;
    private float lerpElapsedTime;
    private float currentOrthoSize;
    private float targetOrthoSize;

    void Awake()
    {
        virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        virtualCamera.m_Lens.OrthographicSize = closeViewOrthoSize;
        currentOrthoSize = targetOrthoSize = closeViewOrthoSize;
        playerMovementModel.OnZoomChange += HandleZoomChange;
    }
    
    void Update()
    {
        if (isZooming)
        {
            if (Mathf.Abs(currentOrthoSize - targetOrthoSize) < 0.001f)
            {
                isZooming = false;
                return;
            }
            lerpElapsedTime = Time.time - lerpStartTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, lerpElapsedTime / lerpTime);
            currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
            
        }
    }

    void HandleZoomChange(bool toCloseView)
    {
        if (isCloseView && !toCloseView)
        {
            isZooming = true;
            isCloseView = false;
            targetOrthoSize = farViewOrthoSize;
            lerpStartTime = Time.time;
        }
        else if (!isCloseView && toCloseView)
        {
            isZooming = true;
            isCloseView = true;
            targetOrthoSize = closeViewOrthoSize;
            lerpStartTime = Time.time;
        }
    }

    void OnDestroy()
    {
        playerMovementModel.OnZoomChange -= HandleZoomChange;
    }
}
