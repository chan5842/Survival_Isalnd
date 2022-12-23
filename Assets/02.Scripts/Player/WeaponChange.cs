using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChange : MonoBehaviour
{
    public enum WeaponType
    {
        AK47, 
        SPAS12,        
        M4A1
    }
    public WeaponType curWeapon = WeaponType.SPAS12;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    public SkinnedMeshRenderer SPAS12;  // 기존 무기 스킨 렌더러
    public MeshRenderer[] Ak47;         // AK47 모델 스킨 렌더러
    public MeshRenderer[] M4A1;         // M4A1 모델 스킨 렌더러
    public Animation anim;
    public bool isHaveM4A1 = false;     // 현재무기가 M4A1인가 확인

    // Update is called once per frame
    void Update()
    {
        // 1번 키를 누르면
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            isHaveM4A1 = false;
            anim.Play("draw");
            // Ak47 렌더러 활성화
            for(int i=0; i<Ak47.Length; i++)
                Ak47[i].enabled = true;  
            SPAS12.enabled = false;
            for (int i = 0; i < M4A1.Length; i++)
                M4A1[i].enabled = false;
            curWeapon = WeaponType.AK47;
            ChangeWeapon();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            isHaveM4A1 = false;
            anim.Play("draw");
            // SPAS12 렌더러 활성화
            for (int i = 0; i < Ak47.Length; i++)
                Ak47[i].enabled = false;
            SPAS12.enabled = true;
            for (int i = 0; i < M4A1.Length; i++)
                M4A1[i].enabled = false;
            curWeapon = WeaponType.SPAS12;
            ChangeWeapon();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            isHaveM4A1 = true;
            anim.Play("draw");
            // M4A1 렌더러 활성화
            for (int i = 0; i < Ak47.Length; i++)
                Ak47[i].enabled = false;
            SPAS12.enabled = false;
            for (int i = 0; i < M4A1.Length; i++)
                M4A1[i].enabled = true;
            curWeapon = WeaponType.M4A1;
            ChangeWeapon();
        }
        //ChangeWeapon();
    }

    void ChangeWeapon()
    {
        //curWeapon = (WeaponType)((int)curWeapon % 3);
        weaponImage.sprite = weaponIcons[(int)curWeapon];
    }
}
