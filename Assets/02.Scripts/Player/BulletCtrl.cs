using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    // Attribute(속성)의 종류 중 하나
    // private 변수여도 Inspector 창에서 표시
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform tr;
    [SerializeField]
    TrailRenderer trail;

    public float speed = 1000f;

    // Start is called before the first frame update
    void Awake()
    {
        // Bullet 오브젝트에 있는 Rigidbody를 가져옴
        // 끌어 가져오는 경우 검색을 하기 때문에 속도 차이가 발생
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        trail = GetComponent<TrailRenderer>();
        // 3초후 오브젝트 자동 제거(메모리 관리를 위해 필수)
        //Destroy(gameObject, 3f);
    }

    private void OnEnable()
    {
        // 발사
        // forward 방향으로 100만큼의 속도로 힘을 가해준다.
        // Vector3.forward로 하면 Global 좌표계 기준으로 나감
        rb.mass = 1f;
        rb.AddForce(tr.forward * speed);
        Invoke("BulletDeActive", 3f);
    }

    void BulletDeActive()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke("BulletDeActive");
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
