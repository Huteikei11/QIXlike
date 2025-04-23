using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProを使用するための名前空間

public class StageNumView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText; // TextMeshProの参照を設定

    // Start is called before the first frame update
    void Start()
    {
        int levelCount = 1; // デフォルト値を1に設定

        try
        {
            // SaveManagerからlevelを取得
            levelCount = SaveManager.Instance.GetLevel();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"SaveManagerからデータを取得できませんでした: {ex.Message}");
            return; // 例外が発生した場合、ゲーム開始処理をスキップ
        }

        // TextMeshProに値を表示
        if (stageText != null)
        {
            stageText.text = $"{levelCount+1}";
        }
        else
        {
            Debug.LogError("TextMeshProUGUIが設定されていません。");
        }
    }
}
