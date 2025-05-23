

using Unity.VisualScripting;

public interface IOnHit
{
    public int OnHit(
        int dmg,                //_[W
        bool crit = false,      //NeBJ
        bool isPenetrate=false, //häÑÊ
        bool isBleed = false,   //¬ARÄ
        bool isDefDown= false,  //häÍ¸
        bool isAtkDown = false, //UÍ¸
        bool isRecover = false  //HPñ
        );
    public void OnHooked(int dmg);
    public bool IsDying();
    public void Initialize(
        string name = "Enemy",
        int hpMax = 3,
        int attackPower = 1,
        int defense = 1,
        string enemyType = "Normal",
        bool hasShield = false,
        int shieldDurability = 0,

        float moveSpeed = 1.0f,
        float attackSpeed = 1.0f);
    
}

