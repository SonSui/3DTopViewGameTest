using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_EnemyA1 : MonoBehaviour
{
    public Material preAttack; //‘Oƒ‚[ƒVƒ‡ƒ“
    public Material realAttack; //UŒ‚

    private MeshRenderer render;

    private float preAtkTime = 0.8f;
    private float lifeTime = 2f;
    private float currTime = 0f;

    private int dmg = 0;
    void Start()
    {
        render = GetComponent<MeshRenderer>();
        render.material = preAttack;
    }

    
    void Update()
    {
        currTime += Time.deltaTime;

        if(currTime > preAtkTime) 
        {
            render.material = realAttack;
        }

        if (currTime > lifeTime) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")&& currTime > preAtkTime)
        {
            other.GetComponent<PlayerControl>().OnHit(dmg);
            Destroy(gameObject);
        }
    }

    public void Initialized(int dmg_)
    {
        dmg = dmg_; 
    }
}
