using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerListener : MonoBehaviour
{
    public float closeViewOrthoSize;
    public float farViewOrthoSize;

    [SerializeField]
    private PlayerMovementModel playerMovementModel;

    private Cinemachine.CinemachineVirtualCamera virtualCamera;
    private bool isZooming = false;
    private bool isCloseView = true;

    private float currentOrthoSize;
    private float targetOrthoSize;

    void Awake()
    {
        virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        playerMovementModel.OnZoomChange += HandleZoomChange;
    }
    
    void Update()
    {
        if (isZooming)
        {
            Debug.Log(currentOrthoSize);
            if (Mathf.Abs(currentOrthoSize - targetOrthoSize) < 0.001f)
            {
                isZooming = false;
                return;
            }
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, Time.deltaTime);
            currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
            
        }
    }

    void HandleZoomChange(bool toCloseView)
    {
        if (isCloseView && !toCloseView)
        {
            isZooming = true;
            isCloseView = false;
        }
        else if (!isCloseView && toCloseView)
        {
            isZooming = true;
            isCloseView = true;
        }
    }

    void OnDestroy()
    {
        playerMovementModel.OnZoomChange -= HandleZoomChange;
    }
}
