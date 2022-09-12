using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeLevel = 3f;// 震动幅度
    public float setShakeTime = 0.5f;   // 震动时间
    public float shakeFps = 45f;    // 震动的FPS

    private bool isShakeCamera = false;// 震动标志

    private float shakeTime = 0.0f;
    public float frameTime = 0.03f;
    public float shakeDelta = 0.005f;

    public static CameraShake instance;

    bool isUseCameraShake = true;
    private void Awake()
    {
        instance = this;
    }

    public void StartShake()
    {
        shakeTime = setShakeTime;
        isShakeCamera = true;
    }

    public void StopShake()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        isShakeCamera = false;
    }

    public void UseCameraShake()
    {
        isUseCameraShake = true;
    }

    public void NotUseCameraShake()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        isUseCameraShake = false;
    }

    void Update()
    {
        if (isShakeCamera && isUseCameraShake)
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0)
                {
                    isShakeCamera = false;
                }
                else
                {
                    frameTime += Time.deltaTime;

                    if (frameTime > 1.0 / shakeFps)
                    {
                        frameTime = 0;
                        Camera.main.rect = new Rect(shakeDelta * (-1.0f + shakeLevel * Random.value), shakeDelta * (-1.0f + shakeLevel * Random.value), 1.0f, 1.0f);
                    }
                }
            }
        }
    }
}