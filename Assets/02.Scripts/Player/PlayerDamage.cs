using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    public Image hpBar;
    public Text hpText;
    [SerializeField]
    float currentHp = 0f;
    [SerializeField]
    float MaxHP = 100f;
    bool isDie;
    GameObject Panel_Screen;
    [SerializeField]
    Image bloodScreen;

    public delegate void PlayerDieHandler();
    //public static event PlayerDieHandler OnPlayerDie;

    private void OnEnable()
    {
        G_Manager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        currentHp = G_Manager.g_Manager.gameData.hp;
        //MaxHP = G_Manager.g_Manager.gameData.hp - MaxHP;
    }
    //private void OnDisable()
    //{
    //    G_Manager.OnItemChange -= UpdateSetup;
    //}

    void Start()
    {
        // Panel_HpBar를 찾아 자식의 컴포넌트를 가져와 초기화
        hpBar = GameObject.Find("Panel_HpBar").transform.GetChild(0).GetComponent<Image>();
        hpText= GameObject.Find("Panel_HpBar").transform.GetChild(1).GetComponent<Text>();
        bloodScreen = GameObject.Find("Canvas_UI").transform.GetChild(4).GetComponent<Image>();
        currentHp = G_Manager.g_Manager.gameData.hp;
        hpBar.color = Color.green;

        // Panel_Screen은 비활성화 되어있기 때문에 부모에서 찾음
        Panel_Screen = GameObject.Find("Canvas_UI").transform.GetChild(9).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDie)
            return;
        if (other.CompareTag("PUNCH"))
        {
            HPUIManager();
            if (currentHp <= 0)
            {
                Panel_Screen.SetActive(true);
                //OnPlayerDie();      // 다른 스크립트에 플레이어가 죽었다는 메시지를 보냄
                isDie = true;
                //3초 후에 PlayerDie함수 실행
                Invoke("PlayerDie", 3f);
            }
                
        }
    }

    void PlayerDie()
    {
        //Debug.Log("사망");
        SceneManager.LoadScene("EndScene");

    }
    
    void HPUIManager()
    {
        StartCoroutine("ShowBloodScreen");
        currentHp -= 5;
        // 현재 체력의 제한 범위를 0~100으로 고정
        currentHp = Mathf.Clamp(currentHp, 0, 100);
        hpBar.fillAmount = (float)currentHp / (float)MaxHP;
        hpText.text = currentHp.ToString() + "/" + MaxHP.ToString();

        if (hpBar.fillAmount <= 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount <= 0.5f)
            hpBar.color = Color.yellow;
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.enabled = true;
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.4f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;
    }
}
