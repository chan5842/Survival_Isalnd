using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterDamage : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AudioSource source;
    public AudioClip deadSound;
    
    public Image hpBar;
    public Canvas HpCanvas;

    public GameObject bloodEffect;
    float hp = 0f;
    float hpMax = 100f;
    public bool isDie;

    MonsterCtrl monsterCtrl;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        bloodEffect = Resources.Load("BloodSprayFX") as GameObject;
        hp = hpMax;
        hpBar.color = Color.green;

        monsterCtrl = GetComponent<MonsterCtrl>();
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
        if(col.gameObject.CompareTag("BULLET"))
        {
            HitAniEffect(col);
            hp -= 25;
            hpBar.fillAmount = (float)hp / (float)hpMax;

            if (hpBar.fillAmount <= 0.5f)
                hpBar.color = Color.yellow;
            if(hpBar.fillAmount <= 0.25f)
                hpBar.color = Color.red;

            if (hp<=0)
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
        HpCanvas.enabled = false;
        agent.isStopped = true;
        source.PlayOneShot(deadSound);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        G_Manager.g_Manager.InkillCount();

        monsterCtrl.AttackCollider(false);

        StartCoroutine(PushPool());
    }

    void Die()
    {
        animator.SetTrigger("doDie");
        GetComponent<CapsuleCollider>().enabled = false;
        isDie = true;
        agent.isStopped = true;
        source.PlayOneShot(deadSound, 1f);
        HpCanvas.enabled = false;
        G_Manager.g_Manager.InkillCount();

        monsterCtrl.AttackCollider(false);

        StartCoroutine(PushPool());
    }

    IEnumerator PushPool()
    {
        yield return new WaitForSeconds(5f);
        this.gameObject.SetActive(false);
        isDie = false;
        HpCanvas.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        monsterCtrl.AttackCollider(true);
        hp = hpMax;
        hpBar.fillAmount = (float)hp / (float)hpMax;
        hpBar.color = Color.green;
    }


}
