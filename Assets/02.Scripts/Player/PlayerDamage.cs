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
        // Panel_HpBar�� ã�� �ڽ��� ������Ʈ�� ������ �ʱ�ȭ
        hpBar = GameObject.Find("Panel_HpBar").transform.GetChild(0).GetComponent<Image>();
        hpText= GameObject.Find("Panel_HpBar").transform.GetChild(1).GetComponent<Text>();
        bloodScreen = GameObject.Find("Canvas_UI").transform.GetChild(4).GetComponent<Image>();
        currentHp = G_Manager.g_Manager.gameData.hp;
        hpBar.color = Color.green;

        // Panel_Screen�� ��Ȱ��ȭ �Ǿ��ֱ� ������ �θ𿡼� ã��
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
                //OnPlayerDie();      // �ٸ� ��ũ��Ʈ�� �÷��̾ �׾��ٴ� �޽����� ����
                isDie = true;
                //3�� �Ŀ� PlayerDie�Լ� ����
                Invoke("PlayerDie", 3f);
            }
                
        }
    }

    void PlayerDie()
    {
        //Debug.Log("���");
        SceneManager.LoadScene("EndScene");

    }
    
    void HPUIManager()
    {
        StartCoroutine("ShowBloodScreen");
        currentHp -= 5;
        // ���� ü���� ���� ������ 0~100���� ����
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
