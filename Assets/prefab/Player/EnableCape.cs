using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCape : MonoBehaviour
{
    public GameObject cape;

    void Start()
    {
        if (cape != null)
        {
            cape.SetActive(true);

            Cloth cloth = cape.GetComponent<Cloth>();
            if (cloth != null)
            {
                cloth.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("cape object is not assigned!");
        }
    }
}
