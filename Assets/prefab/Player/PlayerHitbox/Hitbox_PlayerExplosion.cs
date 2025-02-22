using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class Hitbox_PlayerExplosion : MonoBehaviour
{
    private Collider collider1;

    public float lifeTime = 1.5f;
    private float currTime = 0f;
    
    public ParticleSystem particle_System;
    public GameObject hitEffectPrefab;


    private int damage = 1;
    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // �U�������G���L�^����n�b�V���Z�b�g
    public float resetColliderTime = 0.1f;

    private CameraFollow camera1;
    private PlayerControl player;

    private void Awake()
    {
        collider1 = GetComponent<Collider>();
        
    }
    private void OnEnable()
    {
        
        currTime = 0f;
        // �L��������邽�тɋL�^���N���A����
        hitTargets.Clear();
        if (collider1 != null)
        {
            StartCoroutine(ResetCollider());
        }
        particle_System.Clear(); //�G�t�F�N�g�����Z�b�g
        particle_System.Play();
    }
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > lifeTime)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {

            if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
            {
                // �U���Ώۂ��L�^����
                hitTargets.Add(other);
                IOnHit io = other.gameObject.GetComponent<IOnHit>();
                if (io != null)
                {
                    if (io.IsDying()) return;

                    // �_���[�W��^����
                    int dmg = io.OnHit(damage);
                    camera1.ZoomAndShakeCamera();

                    player.VibrateForDuration(0.2f, 0.3f);

                    // �q�b�g�����ʒu���擾
                    Vector3 contactPoint = other.ClosestPoint(transform.position);

                    // �G�t�F�N�g�𐶐�
                    GameObject effect = Instantiate(hitEffectPrefab, contactPoint, Quaternion.identity);

                    // �G�t�F�N�g�������폜
                    Destroy(effect, 2f);

                }
            }

        }
    }

    public void Initialized(PlayerControl player_,CameraFollow cam,int dmg_)
    {
        player = player_;
        camera1 = cam;
        damage = dmg_;
    }
    private IEnumerator ResetCollider()
    {
        
        yield return new WaitForSeconds(resetColliderTime / GameManager.Instance.playerStatus.GetAttackSpeed());
        collider1.enabled = true;
    }
}
