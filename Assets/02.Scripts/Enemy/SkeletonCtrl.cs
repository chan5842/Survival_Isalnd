using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonCtrl : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    public Transform PlayerTr;
    public Transform SkeletonTr;
    SkeletonDamage s_damage;
    float damping = 10f;

    public float traceDist = 20f;
    public float attackDist = 3f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        s_damage = GetComponent<SkeletonDamage>();
        AttackCollider(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(PlayerTr.position,SkeletonTr.position);

        if (s_damage.isDie)
            return;
        if(dist <= attackDist)
        {
            animator.SetBool("isAttack", true);
            agent.isStopped = true;
            AttackCollider(true);
            Quaternion rot = Quaternion.LookRotation(PlayerTr.position - SkeletonTr.position);
            SkeletonTr.rotation = Quaternion.Slerp(SkeletonTr.rotation, rot, Time.deltaTime * damping);
        }
        else if(dist <= traceDist)
        {
            agent.isStopped = false;
            agent.destination = PlayerTr.position;
            animator.SetBool("isTrace", true);
            animator.SetBool("isAttack", false);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isTrace", false);
        }
    }

    public void AttackCollider(bool isEnable)
    {
        foreach (Collider Col in GetComponentsInChildren<SphereCollider>())
        {
            Col.enabled = isEnable;
        }
    }
}
