using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teki01AnimEventForword : MonoBehaviour
{
    private GameObject parentObject;
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    public void OnBiteAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnBiteAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnBiteAnimeEnd()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnBiteAnimeEnd", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnBombAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnBombAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnIdleAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnIdleAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnDeadAnimeEnd()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnDeadAnimeEnd", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnHitAnimeEnd()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnHitAnimeEnd", SendMessageOptions.DontRequireReceiver);
        }
    }
}
