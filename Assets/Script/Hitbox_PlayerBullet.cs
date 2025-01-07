using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox_PlayerBullet : MonoBehaviour
{
    
    public GameObject hitParticleEffect;
<<<<<<< HEAD
    public GameObject trailEffect;
=======
>>>>>>> origin/main

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); //�U�������G���L�^


    private int damage;
    private float critical;
    private bool isDefensePenetration;

    public float speed = 10f; 
    public float lifeTime = 5f;

<<<<<<< HEAD
    private int ammoPenetration = 0;

=======
>>>>>>> origin/main

    private void OnEnable()
    {
        // �L��������邽�тɋL�^���N���A����
        hitTargets.Clear();
        StartCoroutine(AutoDestroyAfterTime(lifeTime));

    }
    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box:crit:" + critical.ToString() + " DefPen:" + isDefensePenetration.ToString());
        if (other.gameObject.CompareTag("Enemy") && !hitTargets.Contains(other))
        {
            // �L�^
            hitTargets.Add(other);
            // �_���[�W�^����
            other.gameObject.GetComponent<IOnHit>().OnHit(damage);

            // �ڂ���ʒu
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            // �G�t�F�N�g����
            GameObject effect = Instantiate(hitParticleEffect, contactPoint, Quaternion.identity);

            // �����I�ɍ폜
            Destroy(effect, 2f);

<<<<<<< HEAD
            ammoPenetration--;
            if (ammoPenetration < 0)
                Destroy(gameObject);
=======
>>>>>>> origin/main
        }
    }


    public void Initialize(int dmg, float criRate = 0.01f, bool isDefPen = false)
    {
        damage = dmg;
        critical = criRate;
        isDefensePenetration = isDefPen;

    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {
        
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
<<<<<<< HEAD
    private void OnDestroy()
    {
        if (trailEffect != null)
        {
            
            trailEffect.transform.parent = null;

            
            var effectDestroyer = trailEffect.GetComponent<TrailEffectDestroyer>();
            if (effectDestroyer != null)
            {
                effectDestroyer.StartDestroySequence();
            }
        }
    }
=======
>>>>>>> origin/main
}
