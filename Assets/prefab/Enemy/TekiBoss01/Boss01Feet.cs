using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01Feet : MonoBehaviour, IOnHit
{
    public Animator animator;
    public Enemy_Boss01 boss;
    

    public int OnHit(
    int dmg,                //ダメージ
    bool crit = false,      //クリティカル
    bool isPenetrate = false, //防御貫通
    bool isBleed = false,   //流血、燃焼
    bool isDefDown = false,  //防御力減
    bool isAtkDown = false, //攻撃力減
    bool isRecover = false  //HP回復
    )
    {
        int hitDmg = boss.OnHit(dmg, crit, isPenetrate, isBleed, isDefDown, isAtkDown, isRecover);
        if (hitDmg != 0)
        {
            animator.SetTrigger("Hit");
        }
        return hitDmg;
    }
    public void OnHooked(int dmg)
    {
        boss.OnHooked(dmg);
    }
    public bool IsDying()
        { return boss.IsDying(); }
    public void Initialize(
        string name = "Enemy",
        int hpMax = 3,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "Normal",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f)
    { }
}
