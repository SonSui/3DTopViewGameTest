using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teki02AnimEventForword : MonoBehaviour
{
    private GameObject parentObject;
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    public void OnAttackAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttackAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnAttackOver()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAttackOver", SendMessageOptions.DontRequireReceiver);
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
