

using Unity.VisualScripting;

public interface IOnHit
{
<<<<<<< HEAD
    public int OnHit(
=======
    public void OnHit(
>>>>>>> origin/main
        int dmg,                //�_���[�W
        bool crit = false,      //�N���e�B�J��
        bool isPenetrate=false, //�h��ђ�
        bool isBleed = false,   //�����A�R��
        bool isDefDown= false,  //�h��͌�
        bool isAtkDown = false, //�U���͌�
        bool isRecover = false  //HP��
        );
    public void OnHooked(int dmg);
<<<<<<< HEAD
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
=======
>>>>>>> origin/main
}

