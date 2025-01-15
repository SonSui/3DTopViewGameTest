using System.Collections.Generic;
using UnityEngine;

// 敵の生成を管理するスクリプト
public class EnemyGenerator : MonoBehaviour
{
    [System.Serializable]
    public class EnemyConfig
    {
        public GameObject enemyPrefab; // 敵のプレハブ
        public int count; // この種類の敵の生成数
        public int hpMax = 3; // 最大HP
        public int attackPower = 1; // 攻撃力
        public int defense = 1; // 防御力
        public string enemyType = "Normal"; // 敵のタイプ
        public bool hasShield = false; // シールドの有無
        public int shieldDurability = 0; // シールド耐久値
        public float moveSpeed = 1.0f; // 移動速度
        public float attackSpeed = 1.0f; // 攻撃速度
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName; // 波の名前
        public List<EnemyConfig> enemies; // この波で生成する敵の設定リスト
    }

    public List<Wave> waves = new List<Wave>(); // 全ての波のリスト
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>(); // スポーンポイントのリスト

    private int currentWaveIndex = 0; // 現在の波のインデックス
    private int enemiesSpawnedInWave = 0; // 現在の波で生成された敵の数
    private int totalEnemiesInWave = 0; // 現在の波の全敵数
    private int enemiesAlive = 0; // 現在生存している敵の数
    private float deltaTime = 0f; // 時間カウンター

    public float spawnInterval = 1.0f; // 敵を生成する間隔

    void Start()
    {
        // 現在の波の敵数を初期化
        if (waves.Count > 0)
        {
            CalculateTotalEnemiesInWave();
        }
    }

    void Update()
    {
        // 全ての波が終了した場合
        if (currentWaveIndex >= waves.Count)
        {
            // 最後の波の全ての敵が死亡した場合にクリアを呼び出す
            if (enemiesAlive <= 0)
            {
                StageManager.Instance?.StageClear();
            }
            return;
        }

        deltaTime += Time.deltaTime;

        // 一定時間ごとに敵を生成
        if (deltaTime >= spawnInterval && enemiesSpawnedInWave < totalEnemiesInWave)
        {
            SpawnEnemyFromWave();
            deltaTime = 0f;
        }

        // 現在の波の全ての敵が死亡した場合、次の波に移行
        if (enemiesAlive <= 0 && enemiesSpawnedInWave >= totalEnemiesInWave)
        {
            currentWaveIndex++;
            if (currentWaveIndex < waves.Count)
            {
                CalculateTotalEnemiesInWave();
                enemiesSpawnedInWave = 0;
            }
        }
    }

    // 現在の波の総敵数を計算
    private void CalculateTotalEnemiesInWave()
    {
        totalEnemiesInWave = 0;
        foreach (var config in waves[currentWaveIndex].enemies)
        {
            totalEnemiesInWave += config.count;
        }
    }

    // 現在の波から敵を1体生成
    private void SpawnEnemyFromWave()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.GetSpawnedNum() < 1)
            {
                Wave currentWave = waves[currentWaveIndex];
                foreach (var config in currentWave.enemies)
                {
                    if (config.count > 0)
                    {
                        GameObject enemy = Instantiate(config.enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                        enemy.GetComponent<IOnHit>().Initialize(
                            config.enemyPrefab.name,
                            config.hpMax,
                            config.attackPower,
                            config.defense,
                            config.enemyType,
                            config.hasShield,
                            config.shieldDurability,
                            config.moveSpeed,
                            config.attackSpeed
                        );
                        spawnPoint.SpawnEnemy(enemy);
                        config.count--;
                        enemiesSpawnedInWave++;
                        enemiesAlive++;
                        return;
                    }
                }
            }
        }
    }

    // 敵が死亡した場合の処理
    public void EnemyDead(GameObject enemy)
    {
        enemiesAlive--;
        if (currentWaveIndex >= waves.Count && enemiesAlive <= 0)
        {
            StageManager.Instance?.StageClear();
        }
    }
}
