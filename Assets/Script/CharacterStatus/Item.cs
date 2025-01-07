using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("装備データ")]
    public ItemData itemData; // 装備データ（アイテムの属性を管理）

    [Header("ビジュアルエフェクト")]
    [SerializeField] private List<Material> rareMaterials; // レアリティに応じたマテリアルリスト
    private MeshRenderer rareRenderer;                     // メッシュレンダラー
<<<<<<< HEAD
    public Material brightCircle;
=======
>>>>>>> origin/main

    // アニメーション関連
    private float rotSpd = 45f;            // 回転速度
    private Vector3 oriScale;              // 元のスケール
    private float ableTime = 0f;           // スケール変更の経過時間
    private float sizeChangeTime = 2f;     // スケール変更の時間
    private bool isEquipped = false;       // 装備済みフラグ

    private void Update()
    {
        // アイテムを回転
        transform.Rotate(Vector3.up, rotSpd * Time.deltaTime, Space.World);

        // スケール変更アニメーション
        if (transform.localScale != oriScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, oriScale, ableTime / sizeChangeTime);
            ableTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        oriScale = transform.localScale; // 元のスケールを保存
        transform.localScale = Vector3.zero; // スケールをゼロに設定
        ableTime = 0f; // 時間をリセット


        //Test 後で削除
        InitializeItem(itemData);
    }


    public void InitializeItem(ItemData itemData_)
    {
        // 装備データを設定
        this.itemData = itemData_;

        // レアリティを設定（範囲外の場合はデフォルトに設定）
        itemData.rare = (itemData.rare >= 0 && itemData.rare < rareMaterials.Count) ? itemData.rare : 0;

        // メッシュレンダラーを取得または作成
        rareRenderer = GetComponent<MeshRenderer>();
        if (rareRenderer == null)
        {
            Debug.LogWarning("MeshRendererが見つからないため、デフォルトの材質を使用します。");
            rareRenderer = gameObject.AddComponent<MeshRenderer>();
            rareRenderer.material = rareMaterials[0];
        }

        // レアリティに応じたマテリアルを設定
        rareRenderer.material = rareMaterials[itemData.rare];
        Color materialColor = rareMaterials[itemData.rare].color;

        // 子オブジェクトのパーティクルシステムの色を設定
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            var mainModule = ps.main;
            mainModule.startColor = materialColor;
        }

<<<<<<< HEAD
        if (brightCircle != null)
        {
            brightCircle.SetColor("_Color", materialColor);
        }
        else
        {
            Debug.LogWarning("brightCircleマテリアルが設定されていません。");
        }

=======
>>>>>>> origin/main
        // アイテムを有効化
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触し、まだ装備されていない場合
        if (other.gameObject.tag == "Player" && isEquipped == false)
        {
            Debug.Log($"{itemData.itemName} in items");
            GameManager.Instance.EquipItem(itemData); // 装備をGameManagerに追加
            isEquipped = true; // 装備済みフラグを設定
            StartCoroutine(OnEquipped()); // 装備後の処理を開始
        }
    }

    
    private IEnumerator OnEquipped()
    {
        float time = 0f;
        float resizeDuration = 0.5f; // 縮小アニメーションの時間

        // アニメーションを実行
        while (time < resizeDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, time / resizeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // アイテムを削除
        Destroy(gameObject);
    }
}