using System.Collections;
using System.Collections.Generic;

namespace DataInfo
{
    [System.Serializable]
    public class GameData
    {
        public int killCount = 0;
        public float hp = 100f;
        public float damage = 25f;
        public float speed = 3f;

        public List<Item> equipItem = new List<Item>();
    }

    [System.Serializable]
    public class Item
    {
        // 아이템 종류
        public enum ItemType { HP, SPEED, DAMAGE, GRENADE} 
        // 계산 방식
        public enum ItemCalc { VALUE, PERCENT}
        public ItemType itemType;
        public ItemCalc itemCalc;
        public string name;
        public string desc;
        public float value;
    }
}

