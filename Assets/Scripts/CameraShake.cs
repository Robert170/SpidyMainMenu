using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
  private CinemachineVirtualCamera _cam;
  [SerializeField]
  private float _shakeTime= 0.2f;
  [SerializeField]
  private float _shakeIntensity = 1.0f;

  private float timer;

   void Awake()
  {
    _cam = GetComponent<CinemachineVirtualCamera>();
    
  }

  private void Start()
  {
    StopShake();
    timer = _shakeTime;
  }

  public void ShakeCamera()
  {
    CinemachineBasicMultiChannelPerlin _multiChannelPerlin = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    _multiChannelPerlin.m_AmplitudeGain = _shakeIntensity;
  }

  void StopShake()
  {
    CinemachineBasicMultiChannelPerlin _multiChannelPerlin = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    _multiChannelPerlin.m_AmplitudeGain = 0f;

    timer = 0f;
  }

  void Update()
  {
    if (StaticVar.shakeCamera)
    {
      ShakeCamera();
    }

    if (timer > 0f && StaticVar.shakeCamera)
    {
      timer -= Time.deltaTime;

      if (timer <= 0f) { 
        StopShake();
        StaticVar.shakeCamera = false;
        timer = _shakeTime;
      }
    }

  }

}
