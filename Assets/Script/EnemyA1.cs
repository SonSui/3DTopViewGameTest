using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA1 : MonoBehaviour,IOnHit
{
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    public GameObject hitboxPrefab;
    private GameObject hitbox=null;

    private Material oriMaterial;
    private Material temMaterial;

    int hp = 7;
    float speed = 2.0f;
    int dmg = 1;

    float atkInterval = 4f;
    float atkTime = 0f;

    bool isDying = false;

    //�v���C���[�̍��W
    Transform playerT;

    EnemyGenerator enemyGenerator;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);
        enemyGenerator = FindObjectOfType<EnemyGenerator>();

        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        
        if(hitbox==null)
        {
            atkTime += Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, playerT.position) < 3.0f && hitbox == null && atkTime>atkInterval)
        {
            Attack();
        }

        //�v���C���[�Ƃ̋������߂��Ȃ�����ړ����~�߂�
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f){ return; }

        //�v���C���[�Ɍ����Đi��
        transform.position = 
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }


    //��������0.1�b�ԐԂ��Ȃ�
    public void OnHit(int dmg)
    {
        if (isDying) return;

        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
        //��e�A�j���[�V�����ƃG�t�F�N�g

        if (hp < 0)
        {
            enemyGenerator.deadEnemyNum++;
            OnDead();
            
        }
        DeleteHitbox();
        atkTime = 0;
    }
    private IEnumerator ChangeColorTemporarily()
    {
        
        temMaterial.color = hitColor;
        GetComponent<Renderer>().material = temMaterial;

        
        yield return new WaitForSeconds(hitDuration);

        
        temMaterial.color = oriMaterial.color;
        GetComponent<Renderer>().material = temMaterial;
    }
    private void OnDead()
    {
        isDying = true;
        //���S�A�j���[�V�����ƃG�t�F�N�g

        //�A�j���[�V��������������폜
        Destroy(gameObject);
    }
    private void Attack()
    {
        hitbox = Instantiate(hitboxPrefab);
        hitbox.GetComponent<Hitbox_EnemyA1>().Initialized(dmg);
        hitbox.transform.position = transform.position;
        hitbox.transform.SetParent(transform);
        atkTime = 0f;
    }
    private void DeleteHitbox()
    {
        if(hitbox!=null)Destroy(hitbox.gameObject);
    }
}
