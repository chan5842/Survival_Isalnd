using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorOnOff : MonoBehaviour
{
    Image PanelImg;
    float timePreve;

    // 0.3초마다 플레이어의 위치 깜빡이게 하는 스크립트
    void Start()
    {
        PanelImg = GetComponent<Image>();
        timePreve = Time.time;
    }

    private void Update()
    {//
        if(Time.time - timePreve > 0.3f)
        {
            timePreve = Time.time;
            PanelImg.enabled = !PanelImg.enabled;
        }
    }
}
