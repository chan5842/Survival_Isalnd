using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;               // 인공지능 관련 기능

public class ZombieCtrl : MonoBehaviour
{
    public Animator animator;       // 애니메이터 컨트롤러
    public NavMeshAgent agent;      // NavMeshAgent(AI기능 사용)
    public Transform PlayerTr;      // 플레이어의 위치
    public Transform ZombieTr;      // 좀비의 위치

    public float traceDist = 15f;   // 추적 가능 거리
    public float attackDist = 2f;   // 공격 가능 거리

    float damping = 10f;

    ZombiDamage z_damage;

    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Start()
    {
        z_damage = GetComponent<ZombiDamage>();
        PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        AttackCollider(false);
    }

    void Update()
    {
        // 두 캐릭터간의 거리
        
        float dist = Vector3.Distance(PlayerTr.position, ZombieTr.position);
        
        if(!z_damage.isDie)
        {
            if (dist <= attackDist)
            {
                // 공격 가능 범위에 들어오면 애니메이션 실행
                animator.SetBool("isAttack", true);
                agent.isStopped = true;                 // 공격시 추적 중지
                AttackCollider(true);
                Quaternion rot = Quaternion.LookRotation(PlayerTr.position - ZombieTr.position);
                ZombieTr.rotation = Quaternion.Slerp(ZombieTr.rotation, rot, Time.deltaTime * damping);
            }
            else if (dist <= traceDist)
            {
                agent.isStopped = false;                // 추적시 이동 가능
                agent.destination = PlayerTr.position;  // 추적 목적지는 Player의 현 위치
                animator.SetBool("isAttack", false);
                // 추적 가능 범위에 들어오면 애니메이션 실행
                animator.SetBool("isTrace", true);
                //AttackCollider(false);
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("isTrace", false);
               // AttackCollider(false);
            }
        }
       
    }

    public void AttackCollider(bool isEnable)
    {
        foreach (Collider Col in GetComponentsInChildren<SphereCollider>())
        {
            Col.enabled = isEnable;
        }
    }

    public void OnPlayerDie()
    {
        if (z_damage.isDie)
            return;
        Debug.Log("플레이어 사망");
        animator.SetBool("isAttack", false);
        animator.SetBool("isTrace", false);
        AttackCollider(false);
        agent.isStopped = true;
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }

    //private void OnEnable()
    //{
        
    //    PlayerDamage.OnPlayerDie += this.OnPlayerDie;
    //}

    //private void OnDisable()
    //{
    //    PlayerDamage.OnPlayerDie -= this.OnPlayerDie;
    //}
}
