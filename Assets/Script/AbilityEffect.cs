[System.Serializable]
public class AbilityEffect
{
    //yƒNƒŠƒeƒBƒJƒ‹z
    public float critBonus;          //ƒNƒŠƒeƒBƒJƒ‹Šm—¦
    public bool unlockExplosion;   //”š”­
    
    //yUŒ‚z
    public int damage;

    //y‘f‘‚³z
    public float speed;

    //y‘Ì—Íz
    public int life;

    public AbilityEffect()
    {
        this.critBonus = 0f;
        this.unlockExplosion = false;
        this.damage = 0;
        this.speed = 0f;
        this.life = 0;
    }

    //+ ‰‰Zq
    public static AbilityEffect operator +(AbilityEffect a, AbilityEffect b)
    {
        if (a == null && b == null)
            return null;
        if (a == null)
            return b;
        if (b == null)
            return a;

        return new AbilityEffect
        {
            critBonus = a.critBonus + b.critBonus,
            unlockExplosion = a.unlockExplosion || b.unlockExplosion, 
            damage = a.damage + b.damage,
            speed = a.speed + b.speed,
            life = a.life + b.life
        };
    }
}