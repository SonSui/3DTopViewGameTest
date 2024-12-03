using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyA2 : MonoBehaviour , IOnHit
{
    EnemyStatus enemyStatus;
    string name_;
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    public GameObject hitboxPrefab;
    private GameObject hitbox=null;

    private Material oriMaterial;
    private Material temMaterial;

    int hp = 4;

    float speed = 2.0f;
    int dmg = 1;

    float atkInterval = 4f;
    float atkTime = 0f;

    //���S����
    bool isDying = false;

    //�v���C���[�̍��W
    public Transform playerT;

    EnemyGenerator enemyGenerator;
    void Start()
    {
        name_ = "EnemyA2_" + System.Guid.NewGuid().ToString();
        int hp_ = 5;
        int attack_ = 1;

        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);
        enemyGenerator = FindObjectOfType<EnemyGenerator>();

        playerT = GameObject.FindGameObjectWithTag("Player").transform;

        enemyStatus = new EnemyStatus(name_,hp_,attack_);
    }
    private void Update()
    {

        if (hp < 0)
        {
            enemyGenerator.deadEnemyNum++;
            Destroy(gameObject);
        }

        if (hitbox == null)
        {
            atkTime += Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, playerT.position) < 3.0f && hitbox == null && atkTime > atkInterval)
        {
            Attack();
        }

        //�v���C���[�Ƃ̋������߂��Ȃ�����ړ����~�߂�
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f) { return; }

        //�v���C���[�Ɍ����Đi��
        transform.position =
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }
    public void OnHit(int dmg, bool crit = false)
    {
        if (isDying) return;

        hp -= dmg;
        enemyStatus.TakeDamage(dmg);
        StartCoroutine(ChangeColorTemporarily());
        //��e�A�j���[�V�����ƃG�t�F�N�g
        if (enemyStatus.IsDead())
        {
            Debug.Log($"{name_} dead");
            Destroy(gameObject);
        }
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
        hitbox.GetComponent<Hitbox_EnemyA2>().Initialized(dmg);
        hitbox.transform.position = transform.position;
        hitbox.transform.SetParent(transform);
        atkTime = 0f;
    }
    private void DeleteHitbox()
    {
        if (hitbox != null) Destroy(hitbox.gameObject);
    }

}
