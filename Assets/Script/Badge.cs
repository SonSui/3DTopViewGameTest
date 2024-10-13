using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Badge : MonoBehaviour
{
    private AbilityManager abilityManager;

    public Item itemToCollect;

    void Start()
    {
        abilityManager = AbilityManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (itemToCollect != null)
            {
                abilityManager.CollectItem(itemToCollect);
                Destroy(gameObject);
            }
        }
    }
}
