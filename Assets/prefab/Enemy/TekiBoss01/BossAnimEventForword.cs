using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimEventForword : MonoBehaviour
{
    private GameObject parentObject;
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    public void OnAttack1Anime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttack1Anime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnAttack1Over()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttack1Over", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnAttack2Anime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttack2Anime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnAttack2Over()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttack2Over", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnIdleAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnIdleAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnHitAnimeOver()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnHitAnimeOver", SendMessageOptions.DontRequireReceiver);
        }
    }
}
