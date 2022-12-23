using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    public Light blueLight;
    public Light yellowLight;

    void Start()
    {
        TurnOn();        
    }

    void TurnOn()
    {
        // Start 코루틴 호출
        StartCoroutine(LightOnOff());
    }

    // 일정 시간 간격으로 반복 호출
    IEnumerator LightOnOff()
    {
        yellowLight.enabled = false;
        blueLight.enabled = true;
        yield return new WaitForSeconds(3.0f);

        // 3초 후 실행
        blueLight.enabled = false;
        yellowLight.enabled = true;
        yield return new WaitForSeconds(3.0f);
        // 3초 동안 유니티 엔진은 다른 작업을 진행

        TurnOn();
    }
}
