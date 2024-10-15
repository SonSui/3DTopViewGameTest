using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA1 : MonoBehaviour
{
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    private Material oriMaterial;
    private Material temMaterial;
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);

    }


    //åÇÇΩÇÍÇÈÇ∆0.1ïbä‘ê‘Ç≠Ç»ÇÈ
    public void OnHit()
    {
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
