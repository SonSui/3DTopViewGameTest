using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA1 : MonoBehaviour
{
    public Color hitColor = Color.red;

    public float hitDuration = 0.1f;

    private Material oriMaterial;
    private Material temMaterial;

    int hp = 4;
    float speed = 2.0f;

    //�v���C���[�̍��W
    private Transform playerT;

    EnemyGenerator enemyGenerator;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oriMaterial = renderer.material;
        temMaterial = new Material(oriMaterial);
        //enemyGenerator = FindObjectOfType<EnemyGenerator>();

        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (hp < 0) {
            //enemyGenerator.deadEnemyNum++;
            Destroy(gameObject);
        }

        //�v���C���[�Ƃ̋������߂��Ȃ�����ړ����~�߂�
        if (Vector3.Distance(transform.position, playerT.position) < 2.1f)
        return;

        //�v���C���[�Ɍ����Đi��
        transform.position = 
            Vector3.MoveTowards(transform.position, new Vector3(playerT.position.x, playerT.position.y, playerT.position.z), speed * Time.deltaTime);
    }


    //��������0.1�b�ԐԂ��Ȃ�
    public void OnHit(int dmg)
    {
        hp -= dmg;
        StartCoroutine(ChangeColorTemporarily());
    }
    private IEnumerator ChangeColorTemporarily()
    {
        
        temMaterial.color = hitColor;
        GetComponent<Renderer>().material = temMaterial;

        
        yield return new WaitForSeconds(hitDuration);

        
        temMaterial.color = oriMaterial.color;
        GetComponent<Renderer>().material = temMaterial;
    }
}
