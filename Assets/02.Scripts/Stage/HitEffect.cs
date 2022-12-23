using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1.충돌감지 총알은 맞자마자 제거되고 이펙트효과와 소리가 난다
public class HitEffect : MonoBehaviour
{
    public GameObject Spark;    // 총알 타격 이펙트
    public AudioSource source;
    public AudioClip hitSound;  // 피격음

    // isTrigger 체크 안했을 때
    // 충돌시 자동 호출되는 콜백함수(트리거는 통과되지만 이건 막힌다)
    private void OnCollisionEnter(Collision col)
    {
        // 충돌한 오브젝트의 태그가 "BULLET"이라면
        if (col.gameObject.CompareTag("BULLET"))
        {
            // 총알 즉시 제거
            //Destroy(col.gameObject);
            col.gameObject.SetActive(false);
            // 피격음 재생
            SoundManager.soundManager.PlaySound(transform.position, hitSound);
            //source.PlayOneShot(hitSound, 1.0f);
            // 이펙트 효과 발생(2초 후 제거)
            ShowEffect(col);
            //GameObject eff = Instantiate(Spark, col.transform.position, Quaternion.identity);
            //Destroy(eff, 2.5f);
        }
    }
    void OnDamage(object[] _params)
    {
        ShowEffect((Vector3)_params[0]);
        SoundManager.soundManager.PlaySound(transform.position, hitSound);
        //source.PlayOneShot(hitSound);
    }

    private void ShowEffect(Vector3 pos)
    {
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, pos.normalized);

        GameObject spk = Instantiate(Spark, pos, rot);
        Destroy(spk, 2f);
    }

    private void ShowEffect(Collision col)
    {
        // 충돌 지점을 좌표로 추출
        ContactPoint contact = col.contacts[0];
        // 백터가 이루는 회전 각도를 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        GameObject spk = Instantiate(Spark, contact.point, rot);
        Destroy(spk, 2f);
    }
}
