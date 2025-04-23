using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckRation : MonoBehaviour
{
    public float ratio;
    public TextMeshProUGUI areaRatio;
    public float clearRatio = 0.1f; // クリア条件の比率
    public float CalculateTransparencyRatio(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture2Dがnullです。");
            return 0f;
        }

        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int transparentPixels = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a == 0f) // アルファ値が0の場合、透明とみなす
            {
                transparentPixels++;
            }
        }
        ratio = (float)transparentPixels / totalPixels;
        DisplayRatioAsPercent();

        CheckClear();//クリア判定
        return ratio;
    }

    public void DisplayRatioAsPercent()
    {
        if (areaRatio == null)
        {
            Debug.LogWarning("TextMeshProUGUIが設定されていません。");
            return;
        }

        // ratioをパーセント表記に変換
        float percent = ratio * 100f;
        float clearPercent = clearRatio * 100f; // クリア条件のパーセント

        // TextMeshProUGUIに表示
        areaRatio.text = $"{percent:F1}/{clearPercent:F0}"; // 小数点以下2桁まで表示
    }

    private void CheckClear()
    {
        if (ratio > clearRatio)
        {
            Debug.Log("クリア条件を満たしました！");
            //クリア演出

            // クリア処理をここに追加
            try
            {
                // SaveManagerからlevelとlifeを取得して初期化
                int level = SaveManager.Instance.GetLevel();
                int life = SaveManager.Instance.GetLifeCount();

                SaveManager.Instance.SetLevel(level+1);
                SaveManager.Instance.SetLifeCount(life);

                Debug.Log($"ゲームを開始します。 レベル: {level}, ライフ: {life}");
            }
            catch (System.Exception ex)
            {
                Debug.Log($"SaveManagerからデータを取得できませんでした: {ex.Message}");
            }
            // シーンをロード
            SceneManager.LoadScene("Main");
        }
    }
}
