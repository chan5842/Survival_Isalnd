using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;


// 스폰 구현
// 1. 스폰 위치 2. 대상 프리펩 3. 스폰 간격 4. 스폰 수
public class G_Manager : MonoBehaviour
{
    public static G_Manager g_Manager;

    public Transform[] Points;       // 좀비가 스폰될 장소
    public Transform[] m_Points;     // 몬스터가 스폰될 장소
    public Transform[] s_Points;     // 해골이 스폰될 장소
    GameObject zombiePrefab;         // 좀비 프리펩
    GameObject monsterPrefab;        // 몬스터 프리펩
    GameObject skeletonPrefab;       // 몬스터 프리펩
    //GameObject bulletPrefab;         // 플레이어 총알 프리펩

    const int z_maxCount = 10;       // 좀비 최대 개수
    const int m_maxCount = 8;        // 몬스터 최대 개수
    const int s_maxCount = 5;        // 해골 최대 개수
    const int MAX_BULLET = 10;       // 총알 최대 개수

    //public Text KillText;
    //public static int total;
    
    //float timePreve;
    //float m_timePreve;               // 몬스터 스폰 간격 조정을 위한 시간 변수
    //float s_timePreve;               // 해골 스폰 간격 조정을 위한 시간 변수

    public List<GameObject> zombiePool = new List<GameObject>();
    public List<GameObject> monsterPool = new List<GameObject>();
    public List<GameObject> skeletonPool = new List<GameObject>();
    public List<GameObject> bulletPool = new List<GameObject>();

    public bool isPaused = false;

    public CanvasGroup inventoryCG;

    [Header("GameData")]
    public Text killCountText;
    public DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData;

    // 인벤토리 아이템이 변경 되었을 때 발생 시킬 이벤트
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    // SlotList 게임 오브젝트에 저장 할 변수
    [SerializeField]
    GameObject slotList;
    // ItemList 하위에 있는 4개의 아이템을 저장할 배열
    public GameObject[] itemObjects;

    private void Awake()
    {
        if (g_Manager == null)
            g_Manager = this;
        else
            Destroy(this.gameObject);

        //DontDestroyOnLoad(this.gameObject);

        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();
        //OnInventoryOpen(false);
        InitInventory();

        zombiePrefab = Resources.Load("Zombie") as GameObject;
        monsterPrefab = Resources.Load("Monster") as GameObject;
        skeletonPrefab = Resources.Load("Skeleton") as GameObject;
        //bulletPrefab = Resources.Load("Bullet") as GameObject;
        
        // 스폰 지점이 많은 경우 대비
        // SpawnPoint라는 이름의 오브젝트를 Hierachy에서 검색하여 정보를 가져옴
        Points = GameObject.Find("SpawnPoint").
            GetComponentsInChildren<Transform>();
        m_Points = GameObject.Find("m_SpawnPoint").
            GetComponentsInChildren<Transform>();
        s_Points = GameObject.Find("s_SpawnPoint").
            GetComponentsInChildren<Transform>();

        slotList = inventoryCG.transform.Find("SlotList").gameObject;

        LoadGameData();

        OnItemChange();
    }

    void LoadGameData()
    {
        //GameData data = dataManager.Load();
        //gameData.hp = data.hp;
        //gameData.speed = data.speed;
        //gameData.damage = data.damage;
        //gameData.killCount = data.killCount;

        // 보유한 아이템이 있는 경우에만 호출
        if (gameData.equipItem.Count > 0)
        {
            InventorySetup();
        }

        killCountText.text = "KILL : " + gameData.killCount.ToString("000");
    }

    // 로드한 데이터를 기준으로 인벤토리 아이템을 추가하는 함수
    void InventorySetup()
    {
        // SlotList 하위에 오브젝트 Transform 정보를 추출
        var slots = slotList.GetComponentsInChildren<Transform>();
        // 보유한 아이템의 개수만큼 반복
        for (int i = 0; i < gameData.equipItem.Count; i++)
        {
            // 인벤토리 UI에 있는 Slot개수 만큼반복
            for (int j = 1; j < slots.Length; j++)
            {
                // Slot 하위에 다른 아이템이 있으면 다음 인덱스로 넘어감
                if (slots[j].childCount > 0) continue;

                // 보유한 아이템 종류에 따라 인덱스를 추출
                int itemIdx = (int)gameData.equipItem[i].itemType;
                // 아이템의 부모를 Slot으로 변경
                itemObjects[itemIdx].GetComponent<Transform>().SetParent(slots[j]);
                // 아이템의 정보를 itemData에 로드한 데이터 값을 저장
                itemObjects[itemIdx].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                // 아이템을 Slot에 추가하면 바깥 for문으로 빠져나감
                break;
            }
        }
    }

    void Start()
    {
        CreateZombiePool();
        CreateMonsterPool();
        CreateSkeletonPool();
        //CreateBulletPool();

        InvokeRepeating("RepeatingZombie", 2f, 3f);
        InvokeRepeating("RepeatingMonster", 5f, 3f);
        InvokeRepeating("RepeatingSkeleton", 3f, 3f);
    }

    void CreateZombiePool()
    {
        GameObject zombieObj = new GameObject("ZombiePool");  
        for (int i = 0; i < z_maxCount; i++)
        {
            GameObject zombie = Instantiate(zombiePrefab, zombieObj.transform);
            zombie.name = "Zombie" + i.ToString("00");   
            zombie.SetActive(false);               
            zombiePool.Add(zombie);                  
        }
    }
    void CreateMonsterPool()
    {
        GameObject monsterObj = new GameObject("MonsterPool"); 
        for (int i = 0; i < m_maxCount; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, monsterObj.transform);
            monster.name = "Monster" + i.ToString("00"); 
            monster.SetActive(false);              
            monsterPool.Add(monster);                
        }
    }
    void CreateSkeletonPool()
    {
        GameObject skeletonObj = new GameObject("SkeletonPool");
        for (int i = 0; i < s_maxCount; i++)
        {
            GameObject skeleton = Instantiate(skeletonPrefab, skeletonObj.transform);
            skeleton.name = "Skeleton" + i.ToString("00");
            skeleton.SetActive(false);
            skeletonPool.Add(skeleton);
        }
    }

    //void CreateBulletPool()
    //{
    //    GameObject bulletPools = new GameObject("BulletPool");
    //    for (int i = 0; i < MAX_BULLET; i++)
    //    {
    //        GameObject _bullet = Instantiate(bulletPrefab, bulletPools.transform);
    //        _bullet.name = "Bullet" + i.ToString("00"); // 두자릿수로 표현
    //        _bullet.SetActive(false);
    //        bulletPool.Add(_bullet);
    //    }
    //}

    public GameObject GetBullet()
    {
        for(int i=0; i<bulletPool.Count; i++)
        {
            if(bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    void RepeatingZombie()
    {
        foreach(GameObject zombie in zombiePool)
        {
            if(zombie.activeSelf == false)
            {
                int idx = Random.Range(1, Points.Length);
                zombie.transform.position = Points[idx].position;
                zombie.transform.rotation = Points[idx].rotation;
                zombie.SetActive(true);
                break;
            }
        }
    }

    void RepeatingMonster()
    {
        foreach (GameObject monster in monsterPool)
        {
            if (monster.activeSelf == false)
            {
                int idx = Random.Range(1, m_Points.Length);
                monster.transform.position = m_Points[idx].position;
                monster.transform.rotation = m_Points[idx].rotation;
                monster.SetActive(true);
                break;
            }
        }
    }

    void RepeatingSkeleton()
    {
        foreach (GameObject skeleton in skeletonPool)
        {
            if (skeleton.activeSelf == false)
            {
                int idx = Random.Range(1, s_Points.Length);
                skeleton.transform.position = s_Points[idx].position;
                skeleton.transform.rotation = s_Points[idx].rotation;
                skeleton.SetActive(true);
                break;
            }
        }
    }

    void InitInventory()
    {
        inventoryCG.alpha = 0f;
        inventoryCG.interactable = false;
        inventoryCG.blocksRaycasts = false;
    }

    public void OnInventoryOpen(bool isOpen)
    {
        inventoryCG.alpha = isOpen ? 1f : 0f;
        inventoryCG.interactable = isOpen;
        inventoryCG.blocksRaycasts = isOpen;

        OnPausedClick();
        Cursor.visible = isOpen;
        if(isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnPausedClick()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

    public void InkillCount()
    {
        ++gameData.killCount;
        killCountText.text = "KILL : " + "<color=#ff0000>" + gameData.killCount.ToString("000") + "</color>";
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            OnInventoryOpen(!isPaused);
        }
    }

    void SaveGameData()
    {
        //dataManager.Save(gameData);
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    // 인벤토리 아이템을 추가 했을 때 데이터의 정보를 갱신하는 함수
    public void AddItem(Item item)
    {
        // 장착 아이템과 같은 아이템을 얻었다면 추가하지 않음
        if (gameData.equipItem.Contains(item)) return;
        // 아이템을 GameData.equipItem 배열에 추가
        gameData.equipItem.Add(item);
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp += gameData.hp / (1f + item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed / (1f + item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage / (1f + item.value);
                break;
            case Item.ItemType.GRENADE:

                break;
        }
        //.aseet 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);

        // 아이템이 변경 된 것을 실시간으로 적용하기 위해 이벤트를 발생 시킴
        OnItemChange();
    }

    // 인벤토리에서 아이템을 제거 했을 때 데이터를 갱신 하는 함수
    public void RemoveItem(Item item)
    {
        // 아이템을 GameData.equipItem 배열에서 제거
        gameData.equipItem.Remove(item);
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp -= gameData.hp / (1f + item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed -= gameData.speed / (1f + item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage -= gameData.damage / (1f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }

        //.aseet 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);

        // 아이템이 변경 된 것을 실시간으로 적용하기 위해 이벤트를 발생 시킴
        OnItemChange();
    }
    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    //void Update()
    //{
    //    // 현재 시간 - 과거 시간 = 흘러간 시간
    //    // 3초마다 스폰
    //    if (Time.time - timePreve > 1f)
    //    {
    //        timePreve = Time.time;
    //        // 좀비 태그 달린 오브젝트 카운트
    //        int zombieCount = (int)GameObject.FindGameObjectsWithTag("ZOMBIE").Length;
    //        // 최대 개수가 되기 전까지 스폰
    //        if (zombieCount < z_maxCount)
    //            //CreateZombie();
    //            CreateEnemy(zombiePrefab, Points);
    //    }
    //    if (Time.time - m_timePreve > 5f)
    //    {
    //        m_timePreve = Time.time;
    //        int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;
    //        if(monsterCount<m_maxCount)
    //        {
    //            //CreateMonster();
    //            CreateEnemy(monsterPrefab, m_Points);
    //        }
    //    }
    //    if(Time.time - s_timePreve > 3f)
    //    {
    //        s_timePreve = Time.time;
    //        int skeletonCount = (int)GameObject.FindGameObjectsWithTag("SKELETON").Length;
    //        if(skeletonCount < s_maxCount)
    //        {
    //            //CreateSkeleton();
    //            CreateEnemy(skeletonPrefab, s_Points);
    //        }
    //    }
    //}

    //void CreateEnemy(GameObject enemyPrefab, Transform[] spawnPoints)
    //{
    //    int idx = Random.Range(1, spawnPoints.Length);     // 난수값 설정
    //    Instantiate(enemyPrefab, spawnPoints[idx].position,
    //        spawnPoints[idx].rotation);
    //}

    //void CreateZombie()
    //{
    //    int idx = Random.Range(1, Points.Length);     // 난수값 설정
    //    Instantiate(zombiePrefab, Points[idx].position, 
    //        Points[idx].rotation);
    //}
    //void CreateMonster()
    //{
    //    int idx = Random.Range(0, m_Points.Length);     // 난수값 설정
    //    Instantiate(monsterPrefab, m_Points[idx].position,
    //        m_Points[idx].rotation);
    //}

    //void CreateSkeleton()
    //{
    //    int idx = Random.Range(0, m_Points.Length);     // 난수값 설정
    //    Instantiate(monsterPrefab, m_Points[idx].position,
    //        m_Points[idx].rotation);
    //}

    //public void KillCount(int count)
    //{
    //    total += count;
    //    KillText.text = "Kill : " + "<color=#ff0000>" + total.ToString() + "</color>";
    //}
}
