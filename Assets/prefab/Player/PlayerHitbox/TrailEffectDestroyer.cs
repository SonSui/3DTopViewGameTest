using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffectDestroyer : MonoBehaviour
{
    public float effectDuration = 2.0f; 

    public void StartDestroySequence()
    {
        //エフェクト自動削除
        /*var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            var emission = ps.emission;
            emission.enabled = false; 
        }

        */
        Destroy(gameObject, effectDuration);
    }
}
