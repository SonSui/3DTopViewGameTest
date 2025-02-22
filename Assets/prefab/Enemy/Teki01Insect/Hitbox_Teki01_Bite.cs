using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Teki01_Bite : MonoBehaviour
{
    private Collider collider1;

    public float preAtkTime = 0.1f;
    public float lifeTime = 1f;
    public float currTime = 0f;

    private int dmg = 1;
    private bool isHitted = false;

    private void Awake()
    {
        collider1 = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        isHitted = false;
        currTime = 0f;
        collider1.enabled = false;
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Clear(); //エフェクトをリセット
        particleSystem.Play();
    }
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > preAtkTime)
            collider1.enabled = true;
        if (currTime > lifeTime)
            gameObject.SetActive(false);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHitted)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            isHitted = true;
        }
    }

    public void Initialized(int dmg_)
    {
        dmg = dmg_;
    }


}
