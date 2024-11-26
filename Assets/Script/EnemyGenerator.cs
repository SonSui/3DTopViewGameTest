using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject EnemyA1;
    float span = 2.0f;
    float delta = 0;

    //�N���A����ϐ�
    int enemyNumMax = 6;
    int currEnemyNum = 0;
    public int deadEnemyNum = 0;//�e�X�g���߁AEnemyA1��Update�֐����ύX���Ă���


    public GameObject clearUI;
    
    void Start()
    {
        clearUI.SetActive(false);//�N���A�e�L�X�g�������Ȃ��悤�ɂ���
    }

    void Update()
    {
        this.delta += Time.deltaTime;
        if (currEnemyNum < enemyNumMax)//�V�[���̓G���Ǘ�
        {
            if (this.delta > this.span)
            {
                this.delta = 0;
                GameObject go = Instantiate(EnemyA1);
                float px = Random.Range(-15f, 15f);
                float pz = Random.Range(-5f, 5f);
                go.transform.position = new Vector3(px, 0, pz);

                currEnemyNum++;//�G�̋L��
            }
        }
        if(deadEnemyNum>= enemyNumMax)
        {
            clearUI.SetActive(true);//�N���A�e�L�X�g��\������
        }
        
    }
}
