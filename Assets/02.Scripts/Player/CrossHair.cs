using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    Transform tr;
    Image crossHair;

    float startTime;        // 조준점 이미지가 커지기 시작하는 시간
    public float duration = 0.2f;   // 커지는 속도
    public float minSize = 0.5f;     // 최소 크기
    public float maxSize = 0.8f;     // 최대 크기

    Color originColor = new Color(1f, 0f, 1f, 0.8f);  // 초기 색
    public Color gazeColor = Color.red;               // 응시 중인 경우 색
    public bool isGaze;     // 응시 상태인지 확인
    public static CrossHair _crossHair;
    void Start()
    {
        tr = GetComponent<Transform>();
        crossHair = GetComponent<Image>();
        startTime = Time.time;  // 과거 시간
        tr.localScale = Vector3.one * minSize;          // 초기 크기 지정
        crossHair.color = originColor;                  // 초기 색 지정
        _crossHair = this;
    }

    void Update()
    {
        if (isGaze)  //  응시중이라면 조준선이 빨간색으로
        {
            // 지나간 시간/0.2f
            float t = (Time.time - startTime) / duration;
            tr.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, t);
            crossHair.color = gazeColor;
        }
        else    // 응시중이 아니라면 원래대로
        {
            tr.localScale = Vector3.one * minSize;
            crossHair.color = originColor;
            startTime = Time.time;      // startTime 초기화
        }
    }
}
