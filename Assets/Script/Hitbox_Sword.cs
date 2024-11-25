using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;
    
   
    private int damage;
    private float critical;
    private bool isDefensePenetration;


    void Start()
    {
        
    }

    void Update()
    {

        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box:crit:" + critical.ToString() + " DefPen:" + isDefensePenetration.ToString());
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyA1>().OnHit(damage);
            
        }
    }

    public void Initialize(int dmg, float criRate = 0.01f, bool isDefPen = false)
    {
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
        /*Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (isCritical && critMaterial != null)
            {

                renderer.material = critMaterial;

            }
            else if (genMaterial != null)
            {
                renderer.material = genMaterial;
            }
        }*/
    }

}
