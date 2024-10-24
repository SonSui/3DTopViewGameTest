using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA1 : MonoBehaviour
{
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    private Material oriMaterial;
    private Material temMaterial;
    int hp = 100;
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);

    }
    private void Update()
    {
        if (hp < 0) {
            Destroy(gameObject);
        }
    }


    //Œ‚‚½‚ê‚é‚Æ0.1•bŠÔÔ‚­‚È‚é
    public void OnHit(int dmg)
    {
        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
    }
    private IEnumerator ChangeColorTemporarily()
    {
        
        temMaterial.color = hitColor;
        GetComponent<Renderer>().material = temMaterial;

        
        yield return new WaitForSeconds(hitDuration);

        
        temMaterial.color = oriMaterial.color;
        GetComponent<Renderer>().material = temMaterial;
    }
}
