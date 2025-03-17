using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teki03AnimEventForword : MonoBehaviour
{
    private GameObject parentObject;
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    public void OnAtkAnime()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAtkAnime", SendMessageOptions.DontRequireReceiver);
        }
    }
    public void OnAtkAnimeEnd()
    {
        if (parentObject != null)
        {
            parentObject.SendMessage("OnAtkAnimeEnd", SendMessageOptions.DontRequireReceiver);
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
