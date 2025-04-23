using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public int lifeCount = 10;
    public TextMeshProUGUI lifeText;

    private void Start()
    {
        try
        {
            // SaveManagerからライフを取得
            if (SaveManager.Instance != null)
            {
                lifeCount = SaveManager.Instance.GetLifeCount();
            }
            else
            {
                Debug.LogWarning("SaveManagerが見つかりません。デフォルトのライフ数を使用します。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ライフの取得中にエラーが発生しました: {ex.Message}");
        }
        UpdateLifeText();
    }

    public int AddLife(int add)
    {
        lifeCount += add;
        UpdateLifeText();

        try
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetLifeCount(lifeCount);
            }
            else
            {
                Debug.LogWarning("SaveManagerが見つかりません。ライフ数をセーブできませんでした。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ライフのセーブ中にエラーが発生しました: {ex.Message}");
        }

        return lifeCount;
    }

    public void UpdateLifeText()
    {
        lifeText.text = lifeCount.ToString();
    }
}
