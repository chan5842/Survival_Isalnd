using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningGun : MonoBehaviour
{
    public Animation Combat_anim;
    public bool isRunning = false;  // 달리고 있는 상태인지 확인

    void Update()
    {
        // 두 키를 동시에 누르고 있다면
        if(Input.GetKey(KeyCode.LeftShift) && 
            Input.GetKey(KeyCode.W))
        {
            // 접는 모션 애니메이션 실행
            Combat_anim.Play("running");
            isRunning = true;
        }
        // Shift 키를 눌렀다 땟다면
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            // 원래 상태 애니메이션 실행
            Combat_anim.Play("runStop");
            isRunning = false;
        }
    }
}
