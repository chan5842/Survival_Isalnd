using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    //public AudioClip[] reload;
}


public class FireCtrl : MonoBehaviour
{
    public Animation Combat_anim;           // 발사 애니메이션
    public Transform firePos;               // 발사 시작 위치
    public AudioSource source;              // 오디오 소스
    //public AudioClip fireSound;             // 총 발사 소리 클립

    RunningGun runGun;
    WeaponChange weaponChange;

    public Light FlashLight;                // 플래시 라이트 
    public AudioClip flashSound;            // 플래시 켜질 때 소리 클립

    [Header("Megazine UI")]
    public bool isReloading = false;        // 재장전 상태인지 확인
    [SerializeField]
    Text MegaText;
    [SerializeField]
    Image MegaImage;
    [SerializeField]
    AudioClip reloadSound;
    public int maxBullet = 10;
    public int remainingBullet = 10;
    public float damage;

    readonly string monsterTag = "MONSTER";
    readonly string skeletonTag = "SKELETON";
    readonly string zombieTag = "ZOMBIE";
    readonly string barrelTag = "BARREL";
    readonly string wallTag = "WALL";

    public PlayerSfx playerSfx;

    WeaponChange.WeaponType curWeapon;

    [Header("Raycast Auto Fire")]
    int enemyLayer;
    [SerializeField]
    int layerMask;
    [SerializeField]
    bool isFire;
    [SerializeField]
    float nextFire;
    [SerializeField]
    public float fireRate = 0.2f;

    private void OnEnable()
    {
        G_Manager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        damage = G_Manager.g_Manager.gameData.damage;
    }

    //private void OnDisable()
    //{
    //    G_Manager.OnItemChange -= UpdateSetup;
    //}

    private void Start()
    {
        // 자기자신 오브젝트에 있는 스크립트로 초기화
        runGun = GetComponent<RunningGun>();
        weaponChange = GetComponent<WeaponChange>();

        MegaText = GameObject.Find("Canvas_UI").transform.GetChild(3).GetChild(0).
            GetComponent<Text>();
        MegaImage = GameObject.Find("Canvas_UI").transform.GetChild(3).GetChild(2).
            GetComponent<Image>();
        reloadSound = Resources.Load("Sounds/p_reload 1") as AudioClip;

        MegaImage.fillAmount = 1f;
        curWeapon = weaponChange.curWeapon;

        enemyLayer = LayerMask.NameToLayer("Enemy");
        layerMask = 1 << enemyLayer;
    }

    void Update()
    {
        // 광선 디버그
        Debug.DrawRay(firePos.position, firePos.forward * 20, Color.red);

        //if (MouseHover.instance.isUIHover) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit hit;
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            //isFire = true;
            isFire = hit.collider.CompareTag(zombieTag) || 
                hit.collider.CompareTag(monsterTag) ||
                hit.collider.CompareTag(skeletonTag);
        }
        else
            isFire = false;

        // 마우스 왼쪽 버튼을 눌렀다면 / 1: 오른쪽 버튼 / 2: 휠 버튼
        //if (Input.GetMouseButtonDown(0) && !isReloading)
        if (!isReloading && isFire)
        {
            // 달리고 있는 중이라면 함수를 종료
            if (runGun.isRunning) return;

            if (Time.time > nextFire)
            {
                if (weaponChange.isHaveM4A1)
                    StartCoroutine(FastBulletFire());
                else
                    Fire();

                nextFire = Time.time + fireRate;
            }      
        }       
        FlashOnOff();
        //Reload();
    }

    private void FlashOnOff()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 라이트 토글
            FlashLight.enabled = !FlashLight.enabled;
            source.PlayOneShot(flashSound, 1.0f);
        }    
    }

    void Fire()
    {
        if (isReloading)
            return;

        #region Projectile방식
        // Instantiate(오브젝트, 위치, 회전) : 오브젝트 생성 함수
        // 마우스를 누를때 마다 발사 위치에 총알 생성
        // Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        //GameObject _bullet = G_Manager.g_Manager.GetBullet();
        //if (_bullet != null)
        //{
        //    _bullet.transform.position = firePos.position;
        //    _bullet.transform.rotation = firePos.rotation;
        //    _bullet.SetActive(true);
        //}
        #endregion

        // 에너미와 장애물만 총알 충돌 판정
        int layerMask = (1 << LayerMask.NameToLayer("Enemy")) + (1<< LayerMask.NameToLayer("Obstacle"));
        RaycastHit hit;
        if(Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            
            if(hit.collider.tag == monsterTag || 
                hit.collider.tag == zombieTag || 
                hit.collider.tag == skeletonTag)
            {
                object[] _params = new object[2];
                _params[0] = damage;
                _params[1] = hit.point;
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                    SendMessageOptions.DontRequireReceiver);
            }
            if (hit.collider.CompareTag(barrelTag) || hit.collider.CompareTag(wallTag))
            {
                object[] _params = new object[2];
                _params[0] = hit.point;     // 배열의 두번째에 맞은 위치 전달
                _params[1] = firePos.position;  // 발사 위치 전달
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        Combat_anim.Play("fire");
        //source.PlayOneShot(fireSound, 4.0f);
        
        var _sfx = playerSfx.fire[(int)GetComponent<WeaponChange>().curWeapon];
        source.PlayOneShot(_sfx, 4f);
        remainingBullet--;
        MegaImage.fillAmount = (float)remainingBullet / maxBullet;
        UpdateBulletText();
        if (remainingBullet <= 0)
        {
            StartCoroutine("ReloadDelay");
        }
        //++bulletCnt;        // 발사한 총알 수 증가
    }

    //void Reload()
    //{
    //    // 총알 10개 발사시 자동으로 재장전
    //    if (bulletCnt == 10)
    //        StartCoroutine(ReloadDelay());
    //}

    // 3 점사
    IEnumerator FastBulletFire()
    {
        if (isReloading)
            yield break;
        for(int i=0;i<3; i++)
        {
            Fire();
            yield return new WaitForSeconds(0.2f);
            // 10발을 쏘면 재장전을 하기 위해 
            if (isReloading)
                break;
        }
    }

    IEnumerator ReloadDelay()
    {
        // 이미 재장전 중이면 종료
        if (isReloading) yield break;
        isReloading = true;                     // 재장전 상태로 변경
        yield return new WaitForSeconds(0.1f);
        Combat_anim.Stop("fire");
        // 0.3초간 겹치게 하여 부드러운 애니메이션 재생
        Combat_anim.CrossFade("pump2", 0.3f);   
        //bulletCnt = 0;
        yield return new WaitForSeconds(0.7f);
        isReloading = false;                    // 총 발사 가능 상태로 변경
        remainingBullet = maxBullet;
        MegaImage.fillAmount = (float)remainingBullet / maxBullet;
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        MegaText.text = string.Format("<color=#ff0000>{0}</color>/{1}",
            remainingBullet, maxBullet);
    }
}
