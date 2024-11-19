using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_Sword : MonoBehaviour
{
    private Animator animator;


    public Material critMaterial;
    public Material genMaterial;
    public float currTime;
    public const float TIME_MAX = 1.0f;
    private int damage;
    private bool isCritical;
    private bool isExpl;

    public GameObject expl;


    void Start()
    {
        animator = GetComponent<Animator>();
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
        Debug.Log("Box:crit:" + isCritical.ToString() + " expl:" + isExpl.ToString());
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyA1>().OnHit(damage);
            if (isCritical && isExpl)
            {
                Debug.Log("Explo");
                Instantiate(expl, transform.position, Quaternion.identity);
            }
        }
    }

    public void Initialize(int dmg, bool isCri = false, bool isExp = false)
    {
        damage = dmg;
        isCritical = isCri;
        isExpl = isExp;
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
