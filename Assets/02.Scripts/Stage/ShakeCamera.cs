using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public Transform shakeCamera;    
    public bool isShake = false; 
    Vector3 originPos;               
    Quaternion originRotoate;        

    void Awake()
    {
        shakeCamera = GetComponent<Transform>();
        originPos = shakeCamera.localPosition;
        originRotoate = shakeCamera.localRotation;
    }

    public IEnumerator CameraShake(float duration = 0.8f,
        float magitude = 0.5f, float magnitudRot = 0.7f)
    {
        float passTime = 0f;
        while (passTime < duration)
        {
            // 반경이 1인 구체 내부에 3차원좌표값을 불규칙하게 반환(중복수 X)
            Vector3 shakePos = Random.insideUnitSphere;
            // 카메라의 위치를 변경
            shakeCamera.localPosition = shakePos * magitude;
            if (isShake)
            {
                // 0.0~1.0 사이의 연속성이 있는 난수 발생
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudRot, 0f));
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime;
            yield return null;      // 쉐이크를 한 프레임에 동작시킴
        }
        // 카메라를 흔든 후 원래대로 되돌림
        shakeCamera.localPosition = Vector3.Lerp(shakeCamera.localPosition, originPos, Time.deltaTime * 10f);
        shakeCamera.localRotation = Quaternion.Slerp(shakeCamera.localRotation, originRotoate, Time.deltaTime * 15f);
    }
}
