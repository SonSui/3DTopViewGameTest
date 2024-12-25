using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Hitbox_Sword : MonoBehaviour
{
    public Material critMaterial;
    public Material genMaterial;

    public GameObject hitParticleEffect;
    public GameObject defaultTrail;
    public GameObject fireTrail;


    private HashSet<Collider> hitTargets = new HashSet<Collider>(); //�U�������G���L�^


    private int damage;
    private float critical;
    private bool isDefensePenetration;
    private bool isBleed;
    private CameraFollow camera1;


    PlayerControl player;


    private void OnEnable()
    {
        // �L��������邽�тɋL�^���N���A����
        hitTargets.Clear();
        
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box:crit:" + critical.ToString() + " DefPen:" + isDefensePenetration.ToString());
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // �L�^
            hitTargets.Add(other);
            if (other.gameObject.GetComponent<IOnHit>() != null)
            {
                bool crit = Random.Range(0f, 1f) < critical;
                // �_���[�W�^����
                other.gameObject.GetComponent<IOnHit>().OnHit(damage,crit,isDefensePenetration,isBleed);

                camera1.ZoomAndShakeCamera();
                
                 
                player.VibrateForDuration();
                // �ڂ���ʒu
                Vector3 contactPoint = other.ClosestPoint(transform.position);

                // �G�t�F�N�g����
                GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

                // �����I�ɍ폜
                Destroy(effect, 2f);
            }

        }
    }
    

    public void Initialize(CameraFollow camera_,int dmg, int type = 0,float criRate = 0.01f, bool isDefPen = false)
    {
        camera1 = camera_;
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;
        switch(type)
        {
            case 0:SetDefaultTrail();break;
            case 1:SetFireTrail();break;
        }
        
    }
    private void SetDefaultTrail()
    {
        isBleed = false;
        defaultTrail.SetActive(true);
        fireTrail.SetActive(false);
    }
    private void SetFireTrail()
    {
        isBleed = true;
        defaultTrail.SetActive(false );
        fireTrail.SetActive(true);
    }

}
