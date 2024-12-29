

using Unity.VisualScripting;

public interface IOnHit
{
    public void OnHit(
        int dmg,                //ダメージ
        bool crit = false,      //クリティカル
        bool isPenetrate=false, //防御貫通
        bool isBleed = false,   //流血、燃焼
        bool isDefDown= false,  //防御力減
        bool isAtkDown = false, //攻撃力減
        bool isRecover = false  //HP回復
        );
    public void OnHooked(int dmg);
}

