

using Unity.VisualScripting;

public interface IOnHit
{
    public void OnHit(
        int dmg,                //_[W
        bool crit = false,      //NeBJ
        bool isPenetrate=false, //häÑÊ
        bool isBleed = false,   //¬ARÄ
        bool isDefDown= false,  //häÍ¸
        bool isAtkDown = false, //UÍ¸
        bool isRecover = false  //HPñ
        );
    public void OnHooked(int dmg);
}

