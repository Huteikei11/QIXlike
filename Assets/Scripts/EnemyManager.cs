using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs; // Inspectorで設定する敵のプレハブリスト

    [SerializeField]
    private Vector2 spawnAreaMin; // スポーンエリアの最小座標 (画面内の左下)
    [SerializeField]
    private Vector2 spawnAreaMax; // スポーンエリアの最大座標 (画面内の右上)

    public int difficult = 0;
    public int level = 0;

    private void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("敵のプレハブリストが設定されていません。");
            return;
        }


        try
        {
            // SaveManagerからdifficultとlevelを取得
            difficult = SaveManager.Instance.GetDifficult();
            level = SaveManager.Instance.GetLevel();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManagerからデータを取得できませんでした。初期値を使用します。エラー: {ex.Message}");
        }

        Debug.Log($"難易度: {difficult}, レベル: {level}");

        SpawnFirst(difficult, level); // 初期スポーン
    }

    private void SpawnFirst(int difficult, int level)
    {
        for (int i = 1; i < level+3; i++) 
        {
            int enemynumber = 0;
            if(difficult==0)
            {
                if(i%4== 0)
                {
                    enemynumber = 1;
                }
            }
            else if (difficult == 1)
            {
                if (i % 3 == 0)
                {
                    enemynumber = 1;
                }
            }
            else if (difficult == 2)
            {
                if (i % 2 == 0)
                {
                    enemynumber = 1;
                }
            }
            SpawnEnemy(enemynumber); 
        }
    }

    public void SpawnEnemy(int num)
    {
        // 敵をスポーン
        GameObject enemyPrefab = enemyPrefabs[num]; // リストの指定された番号を使用
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // InstantiateしてSetActive(true)を設定
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemy.SetActive(true); // 非表示のコピー元に対応

        Debug.Log($"敵をスポーンしました: {enemyPrefab.name} at {spawnPosition}");
    }

}
