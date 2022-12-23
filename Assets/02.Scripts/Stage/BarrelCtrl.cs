using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    [SerializeField]
    Texture[] textures;
    MeshRenderer meshRenderer;
    [SerializeField]
    GameObject expEffect;
    //AudioSource audio;
    [SerializeField]
    AudioClip expSound;
    ShakeCamera shakeCamera;

    public delegate void EnemyDieHandler();
    public static event EnemyDieHandler OnEnemyDie;

    public int hitCount = 0;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        textures = Resources.LoadAll<Texture>("BarrelTexture");
        expEffect = Resources.Load("Effects/BigExplosionEffect") as GameObject;
        //audio = GetComponent<AudioSource>();
        expSound = Resources.Load("Sounds/missile_explosion") as AudioClip;
        shakeCamera = Camera.main.GetComponent<ShakeCamera>();

        int idx = Random.Range(0, textures.Length);
        meshRenderer.material.mainTexture = textures[idx];
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("BULLET"))
        {
            if(++hitCount == 3)
            {
                BarrelExplosion();
            }
        }
    }

    void OnDamage(object[] _params)
    {
        Vector3 hitPos = (Vector3)_params[0];
        Vector3 firePos = (Vector3)_params[1];

        Vector3 incomVector = (hitPos - firePos).normalized;
        GetComponent<Rigidbody>().AddForceAtPosition(incomVector * 5000f, hitPos);

        if(++hitCount == 3)
        {
            BarrelExplosion();
        }
    }

    void BarrelExplosion()
    {
        GameObject Effect = Instantiate(expEffect, transform.position, transform.rotation);
        Destroy(Effect, 1f);
        SoundManager.soundManager.PlaySound(transform.position, expSound);
        //audio.PlayOneShot(expSound, 1f);

        Collider[] Cols = Physics.OverlapSphere(transform.position, 1000f);

        foreach (Collider col in Cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if(col.gameObject.tag != "Player")
                {
                    rb.mass = 1f;
                    rb.AddExplosionForce(1000f, transform.position, 20f, 1000f);
                    //col.gameObject.SendMessage("expDie", SendMessageOptions.DontRequireReceiver);
                    OnEnemyDie();
                }
            }
        }
        StartCoroutine(shakeCamera.CameraShake());
        Invoke("BareelNormalMass", 3f);
        Destroy(gameObject, 3f);
    }

    void BareelNormalMass()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, 100f);

        foreach (Collider col in Cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            // 충돌체에 Rigidbody가 존재한다면
            if (rb != null)
            {
                // 플레이어를 제외한 오브젝트에만 영향
                if (col.gameObject.tag != "Player")
                {
                    rb.mass = 100f;       // 무게를 가볍게
                }
            }
        }
    }
}
