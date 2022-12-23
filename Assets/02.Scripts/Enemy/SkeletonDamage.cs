using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SkeletonDamage : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    GameObject bloodEffect;

    AudioSource source;
    public AudioClip deadSound;

    public bool isDie = false;
    float hp = 0f;
    float hpMax = 150f;
    public Image hpBar;
    public Canvas hpCanvas;

    SkeletonCtrl skeletonCtrl;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        bloodEffect = Resources.Load("BloodSprayFX") as GameObject;
        hp = hpMax;
        hpBar.color = Color.green;

        skeletonCtrl = GetComponent<SkeletonCtrl>();
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
        if (col.gameObject.CompareTag("BULLET"))
        {
            HitAniEffect(col);
            hp -= 25f;
            hpBar.fillAmount = (float)hp / (float)hpMax;

            if (hpBar.fillAmount <= 0.5f)
                hpBar.color = Color.yellow;
            if (hpBar.fillAmount <= 0.25f)
                hpBar.color = Color.red;

            if (hp <= 0)
            {
                Die();
            }
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
        source.PlayOneShot(deadSound);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;

        G_Manager.g_Manager.InkillCount();
        skeletonCtrl.AttackCollider(false);

        StartCoroutine(PushPool());
    }
    void Die()
    {
        animator.SetTrigger("doDie");
        GetComponent<CapsuleCollider>().enabled = false;
        agent.isStopped = true;
        isDie = true;
        hpCanvas.enabled = false;
        source.PlayOneShot(deadSound);

        G_Manager.g_Manager.InkillCount();

        skeletonCtrl.AttackCollider(false);

        //foreach (Collider Col in GetComponentsInChildren<SphereCollider>())
        //{
        //    Col.enabled = false;
        //}

        StartCoroutine(PushPool());
    }

    IEnumerator PushPool()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
        //agent.isStopped = false;
        isDie = false;
        hpCanvas.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        skeletonCtrl.AttackCollider(true);
        hp = hpMax;
        hpBar.fillAmount = (float)hp / (float)hpMax;
        hpBar.color = Color.green;
    }
}
