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
}
