using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorOnOff : MonoBehaviour
{
    Image PanelImg;
    float timePreve;

    // 0.3�ʸ��� �÷��̾��� ��ġ �����̰� �ϴ� ��ũ��Ʈ
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
