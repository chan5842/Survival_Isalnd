using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ZombiDamage : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    [SerializeField]
    GameObject bloodEffect;      // 피격 이펙트

    AudioSource source;
    public AudioClip deadClip;

    public bool isDie = false;
    float hp = 0f;                         // 현재 체력
    float  hpMax = 100f;                    // 최대 체력
    public Image hpBar;                 // 체력바 UI
    public Canvas hpCanvas;             // 체력바 Canvas

    ZombieCtrl zombieCtrl;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        bloodEffect = Resources.Load("BloodSprayFX") as GameObject;
        hp = hpMax;
        hpBar.color = Color.green;

        zombieCtrl = GetComponent<ZombieCtrl>();
    }

    private void OnEnable()
    {
        BarrelCtrl.OnEnemyDie += this.expDie;
    }
    private void OnDisable()
    {
        BarrelCtrl.OnEnemyDie -= this.expDie;
    }

    private void OnCollisionEnter(Collision col)
    {
        // 총알이 충돌하면
        if(col.gameObject.CompareTag("BULLET"))
        {
            HitAniEffect(col);
            // 체력감소
            hp -= 25;
            hpBar.fillAmount = (float)hp / (float)hpMax;

            if(hpBar.fillAmount <= 0.5f)
                hpBar.color = Color.yellow;
            if (hpBar.fillAmount <= 0.25f)
                hpBar.color = Color.red;
            // 체력 0이하면 사망
            if (hp <= 0)
                Die();
        }
    }

    void OnDamage(object[] _params)
    {
        Vector3 pos = (Vector3)_params[1];
        HitAniEffect(pos);
        hp -= (float)_params[0];
        hpBar.fillAmount = (float)hp / (float)hpMax;

        if (hpBar.fillAmount <= 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount <= 0.5f)
            hpBar.color = Color.yellow;

        if (hp <= 0)
        {
            Die();
        }
    }

    void HitAniEffect(Vector3 pos)
    {
        ShowBloodEffect(pos);
        animator.SetTrigger("doHit");
        agent.isStopped = true;
    }

    void ShowBloodEffect(Vector3 pos)
    {
        Quaternion rot = Quaternion.LookRotation(-pos.normalized);  // 바라보는 반대쪽으로 혈흔 발생
        GameObject hitEffect = Instantiate<GameObject>(bloodEffect, pos, rot);

        Destroy(hitEffect, 1f);
    }

    private void HitAniEffect(Collision col)
    {
        // 총알 삭제
        //Destroy(col.gameObject);
        col.gameObject.SetActive(false);
        ShowBloodEffect(col);
        // doHit 트리거 변수 실행
        animator.SetTrigger("doHit");
        agent.isStopped = true;
        //// 피격 이펙트 효과
        //GameObject blood = Instantiate(bloodEffect,
        //    col.transform.position, Quaternion.identity);
        //Destroy(blood, 0.5f);
    }

    private void ShowBloodEffect(Collision col)
    {
        ContactPoint contact = col.contacts[0]; // 피격 위치 정보 저장
        Quaternion rot = Quaternion.LookRotation(-contact.normal);  // 바라보는 반대쪽으로 혈흔 발생
        GameObject hitEffect = Instantiate<GameObject>(bloodEffect, contact.point, rot);

        Destroy(hitEffect, 1f);
    }

    public void expDie()
    {
        if (isDie) return;
        animator.SetTrigger("expDie");
        isDie = true;
        hpCanvas.enabled = false;
        agent.isStopped = true;
        source.PlayOneShot(deadClip);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;

        G_Manager.g_Manager.InkillCount();
        zombieCtrl.AttackCollider(false);

        StartCoroutine(PushPool());
    }

    void Die()
    {
        // 사망 애니메이션 실행
        animator.SetTrigger("doDie");
        isDie = true;
        hpCanvas.enabled = false;
        agent.isStopped = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        source.PlayOneShot(deadClip);

        G_Manager.g_Manager.InkillCount();
        // 몹 사망시 자식 컴포넌트들의 콜라이더 비활성화
        zombieCtrl.AttackCollider(false);
        StartCoroutine(PushPool());
    }

    IEnumerator PushPool()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
        isDie = false;
        //agent.isStopped = false;
        hpCanvas.enabled = true;
        zombieCtrl.AttackCollider(true);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        
        hp = hpMax;
        hpBar.fillAmount = (float)hp / (float)hpMax;
        hpBar.color = Color.green;
    }
}
