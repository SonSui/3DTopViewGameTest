

using Unity.VisualScripting;

public interface IOnHit
{
    public void OnHit(
        int dmg,                //�_���[�W
        bool crit = false,      //�N���e�B�J��
        bool isPenetrate=false, //�h��ђ�
        bool isBleed = false,   //�����A�R��
        bool isDefDown= false,  //�h��͌�
        bool isAtkDown = false, //�U���͌�
        bool isRecover = false  //HP��
        );
    public void OnHooked(int dmg);
}

