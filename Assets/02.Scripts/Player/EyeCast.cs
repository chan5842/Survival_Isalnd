using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCast : MonoBehaviour
{
    Transform tr;
    Ray ray;             // 광선
    RaycastHit rayHit;   // 광선의 충돌감지 자료형
    public float dist = 15f;    // 광선 감지 범위

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        ray = new Ray(tr.position, tr.forward * dist);    // 광선 동적할당
        // Scene에서 광선을 확인
        Debug.DrawRay(ray.origin, ray.direction * dist, Color.green);
        // 광선이 충돌 했다면(충돌감지 범위안에서)
        if (Physics.Raycast(ray, out rayHit, dist, 1 << 8 | 1 << 9 | 1 << 10))
        {
            CrossHair._crossHair.isGaze = true; // 응시중 상태로 변경
        }
        else
            CrossHair._crossHair.isGaze = false; // 기본 상태로 변경
    }
}
