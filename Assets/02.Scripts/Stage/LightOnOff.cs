using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    // 다른 스크립에서 사용할 수 있는 Light자료형의 InLight
    public Light InLight;
    //public AudioSource audioSource;
    public AudioClip OnSound;
    public AudioClip OffSound;

    // isTrigger 체크시 통과하면서 충돌 감시가 시작되었을때(닿았을 때)
    // 자동으로 호출되는 CallBack 함수
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상의 태그가 "Player" 라면
        if(other.gameObject.tag == "Player")
        {
            // Light 컴포넌트를 활성화
            InLight.enabled = true;
            // OnSound 클립을 1.0f볼륨으로 재생
            // 효과음 겹치게 들리도록 PlayOneShot함수 사용
            SoundManager.soundManager.PlaySound(transform.position, OnSound);
            //audioSource.PlayOneShot(OnSound, 1.0f);
        }
    }

    //isTrigger 체크시 통과하면서 충돌 감시에서 빠져나갔을 때(벗어났을 때)
    // 자동으로 호출되는 CallBack 함수
    private void OnTriggerExit(Collider other)
    {
        // 충돌했던 대상의 태그가 "Player" 라면
        if(other.gameObject.tag == "Player")
        {
            // Light 컴포넌트 비활성화
            InLight.enabled = false;
            // OffSound 클립 재생
            //audioSource.PlayOneShot(OffSound, 1.0f);
            SoundManager.soundManager.PlaySound(transform.position, OffSound);
        }
    }
}
