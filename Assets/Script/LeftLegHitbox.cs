using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftLegHitbox : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;
    private float currTime;
    public const float TIME_MAX = 0.8f;
    private int damage;
    private bool isCritical;


    void Start()
    {
        currTime = 0f;
    }

    
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > TIME_MAX)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            
            other.gameObject.GetComponent<EnemyA1>().OnHit(damage);
            if (isCritical)
            {
                Debug.Log("Critical");
            }
        }
    }

    public void Initialize(int dmg, bool isCri = false)
    {
        damage = dmg;
        isCritical = isCri;
        Renderer renderer = GetComponent<Renderer>();
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
        }
    }
}
