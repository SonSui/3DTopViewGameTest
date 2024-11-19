using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [SerializeField]
    public  int life;
    public int attackPoint;

    public float speed;
    public int damage;
    public float crit;

    public bool isExplo;
    public CharacterStatus(int life, int damage, float speed, float crit)
    {
        this.life = life;
        this.speed = speed;
        this.damage = damage;
        this.crit = crit;
        this.isExplo = false;
    }
    public void SetFullState(int life, int damage, float speed, float crit, bool explo)
    {
        this.life = life;
        this.speed = speed;
        this.damage = damage;
        this.crit = crit;
        this.isExplo = explo;
    }
    public void UpdateState(int life, int damage, float speed, float crit)
    {
        this.life += life;
        this.speed += speed;
        this.damage += damage;
        this.crit += crit;
    }

    public void UpdateAblitiy(bool explo)
    {
        this.isExplo = explo;
    }
    public void ResetState()
    {
        this.life = 0;
        this.speed = 0;
        this.damage = 0;
        this.crit = 0;
    }
    public void ResetAbility()
    {
        this.isExplo = false;
    }
    public string ShowState()
    {
        string st =
            "life:" + this.life.ToString() + "\n" +
            "speed:" + this.speed.ToString() + "\n" +
            "damage:" + this.damage.ToString() + "\n" +
            "critical:" + this.crit.ToString() + "\n" +
            "exploAbility:" + this.isExplo.ToString() + "\n";
        Debug.Log(st);
        return st;
    }

    public void TakeDamage(int damage)
    {
        this.life -= damage;
    }

    public bool IsDead()
    { 
        return this.life <= 0; 
    }
    public static CharacterStatus operator +(CharacterStatus a, CharacterStatus b)
    {
        return new CharacterStatus(
            a.life + b.life,
            a.damage + b.damage,
            a.speed + b.speed,
            a.crit + b.crit
        )
        { isExplo = a.isExplo || b.isExplo }; 
    }

    
    public void CopyTo(CharacterStatus target)
    {
        target.life = this.life;
        target.speed = this.speed;
        target.damage = this.damage;
        target.crit = this.crit;
        target.isExplo = this.isExplo;
    }
}
