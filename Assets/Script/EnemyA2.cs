using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA2 : MonoBehaviour , IOnHit
{

    EnemyStatus enemyStatus;
    string name_;
    

    // Start is called before the first frame update
    void Start()
    {
        name_ = "EnemyA2_" + System.Guid.NewGuid().ToString();
        int hp_ = 5;
        int attack_ = 1;

        enemyStatus = new EnemyStatus(name,hp_,attack_);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit(int dmg)
    {
        Debug.Log("Enemy2 onhit");
        enemyStatus.TakeDamage(dmg);
        
        if(enemyStatus.IsDead())
        {
            Debug.Log($"{name_} dead");
            Destroy(gameObject);
        }
    }
}
