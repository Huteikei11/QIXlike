using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs; // Inspector�Őݒ肷��G�̃v���n�u���X�g

    [SerializeField]
    private Vector2 spawnAreaMin; // �X�|�[���G���A�̍ŏ����W (��ʓ��̍���)
    [SerializeField]
    private Vector2 spawnAreaMax; // �X�|�[���G���A�̍ő���W (��ʓ��̉E��)

    public int difficult = 0;
    public int level = 0;

    private void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("�G�̃v���n�u���X�g���ݒ肳��Ă��܂���B");
            return;
        }


        try
        {
            // SaveManager����difficult��level���擾
            difficult = SaveManager.Instance.GetDifficult();
            level = SaveManager.Instance.GetLevel();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManager����f�[�^���擾�ł��܂���ł����B�����l���g�p���܂��B�G���[: {ex.Message}");
        }

        Debug.Log($"��Փx: {difficult}, ���x��: {level}");

        SpawnFirst(difficult, level); // �����X�|�[��
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
        // �G���X�|�[��
        GameObject enemyPrefab = enemyPrefabs[num]; // ���X�g�̎w�肳�ꂽ�ԍ����g�p
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // Instantiate����SetActive(true)��ݒ�
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemy.SetActive(true); // ��\���̃R�s�[���ɑΉ�

        Debug.Log($"�G���X�|�[�����܂���: {enemyPrefab.name} at {spawnPosition}");
    }

}
