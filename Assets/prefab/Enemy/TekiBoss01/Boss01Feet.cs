using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01Feet : MonoBehaviour, IOnHit
{
    public Animator animator;
    public Enemy_Boss01 boss;
    

    public int OnHit(
    int dmg,                //�_���[�W
    bool crit = false,      //�N���e�B�J��
    bool isPenetrate = false, //�h��ђ�
    bool isBleed = false,   //�����A�R��
    bool isDefDown = false,  //�h��͌�
    bool isAtkDown = false, //�U���͌�
    bool isRecover = false  //HP��
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
