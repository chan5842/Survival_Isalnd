using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public Transform PlayerTr;
    public Transform MonsterTr;

    float traceDist = 15f;
    float attackDist = 2.5f;

    float damping = 10f;        // 플레이어를 향해 회전하는 계수

    MonsterDamage m_damage;

    private void Start()
    {
        m_damage = GetComponent<MonsterDamage>();
        PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        AttackCollider(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(PlayerTr.position, MonsterTr.position);

        if(!m_damage.isDie)
        {
            if (dist <= attackDist)
            {
                animator.SetBool("isAttack", true);
                agent.isStopped = true;
                AttackCollider(true);
                Quaternion rot = Quaternion.LookRotation(PlayerTr.position - MonsterTr.position);
                MonsterTr.rotation = Quaternion.Slerp(MonsterTr.rotation, rot, Time.deltaTime * damping);
            }
            else if (dist <= traceDist)
            {
                agent.isStopped = false;
                agent.destination = PlayerTr.position;
                animator.SetBool("isAttack", false);
                animator.SetBool("isTrace", true);

            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("isTrace", false);
            }
        }
       
    }

    public void AttackCollider(bool isEnable)
    {
        foreach(Collider Col in GetComponentsInChildren<SphereCollider>())
        {
            Col.enabled = isEnable;
        }
    }
}
