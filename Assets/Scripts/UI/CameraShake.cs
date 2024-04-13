using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    // Shake parameters
    private float shakeDuration = 0f;
    private float shakeAmplitude = 5f;
    private float shakeFrequency = 2.0f;
    private float dampingSpeed = 1.0f;

    private float initialAmplitude;
    private float initialFrequency;

    private static CameraShake instance;

    void Awake()
    {
        instance = this;
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            initialAmplitude = noise.m_AmplitudeGain;
            initialFrequency = noise.m_FrequencyGain;
        }
    }

    public static CameraShake Instance
    {
        get { return instance; }
    }

    public void ShakeCamera(float duration)
    {
        shakeDuration = duration;
        if (noise != null)
        {
            CancelInvoke("StopShaking");
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;
            InvokeRepeating("StopShaking", shakeDuration, 0.01f);
        }
    }

    private void StopShaking()
    {
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime * dampingSpeed;
            noise.m_AmplitudeGain = Mathf.Lerp(0, shakeAmplitude, shakeDuration / initialAmplitude);
            noise.m_FrequencyGain = Mathf.Lerp(0, shakeFrequency, shakeDuration / initialFrequency);
        }
        else
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
            CancelInvoke("StopShaking");
        }
    }
}