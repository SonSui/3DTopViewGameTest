using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA2 : MonoBehaviour , IOnHit
{
    EnemyStatus enemyStatus;
    string name_;
    void Start()
    {
        name_ = "EnemyA2_" + System.Guid.NewGuid().ToString();
        int hp_ = 5;
        int attack_ = 1;

        enemyStatus = new EnemyStatus(name_,hp_,attack_);
    }
    public void OnHit(int dmg, bool crit = false)
    {
        enemyStatus.TakeDamage(dmg);
        
        if(enemyStatus.IsDead())
        {
            Debug.Log($"{name_} dead");
            Destroy(gameObject);
        }
    }
}
