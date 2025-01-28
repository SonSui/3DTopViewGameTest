using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_EnemyA2 : MonoBehaviour
{
    public Material preAttack; //‘Oƒ‚[ƒVƒ‡ƒ“
    public Material realAttack; //UŒ‚

    private MeshRenderer render;
    private Collider collider1;

    private float preAtkTime = 0.8f;
    private float lifeTime = 2f;
    private float currTime = 0f;

    private int dmg = 0;
    private bool isHitted = false;
    void Start()
    {
        render = GetComponent<MeshRenderer>();
        collider1 = GetComponent<Collider>();
        render.material = preAttack;
    }

    
    void Update()
    {
        currTime += Time.deltaTime;

        if(currTime > preAtkTime) 
        {
            collider1.enabled = false;
            render.material = realAttack;
            collider1.enabled = true;
        }

        if (currTime > lifeTime) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")&& currTime > preAtkTime&&!isHitted)
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
