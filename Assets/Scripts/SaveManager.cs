using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveKey = "GameSaveData"; // セーブデータのキー

    // セーブデータ構造体
    [System.Serializable]
    public class SaveData
    {
        public bool[] stage = new bool[7]; // ステージクリア情報 (7ステージ分)
        public int volume = 5; // 音量 (0~9)
        public bool mute = false; // ミュート状態
        public int difficult = 0; // 難易度 (0: Easy, 1: Normal, 2: Hard)
        public bool cheatMode = false; // チートモード
    }

    private SaveData saveData = new SaveData();

    public void Start()
    {
        // 初期化時にセーブデータをロード
        Debug.Log("セーブデータをロードします。");
        LoadData();
    }
    // セーブデータをロード
    public void LoadData()
    {
        try
        {
            if (ES3.KeyExists(SaveKey)) // セーブデータが存在するか確認
            {
                saveData = ES3.Load<SaveData>(SaveKey);
                Debug.Log("セーブデータをロードしました。");
            }
            else
            {
                Debug.LogWarning("セーブデータが存在しません。新しいデータを作成します。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("セーブデータのロード中にエラーが発生しました: " + ex.Message);
        }
    }

    // セーブデータを保存
    public void SaveGameData()
    {
        ES3.Save(SaveKey, saveData);
        Debug.Log("セーブデータを保存しました。");
    }

    // ステージクリア情報を更新
    public void SetStageClear(int stageIndex, bool isClear)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            saveData.stage[stageIndex] = isClear;
            SaveGameData(); // 更新後に保存
        }
        else
        {
            Debug.LogError("無効なステージインデックスです: " + stageIndex);
        }
    }

    // 音量を設定
    public void SetVolume(int volume)
    {
        saveData.volume = Mathf.Clamp(volume, 0, 9); // 0~9の範囲に制限
        SaveGameData(); // 更新後に保存
    }

    // ミュート状態を設定
    public void SetMute(bool isMute)
    {
        saveData.mute = isMute;
        SaveGameData(); // 更新後に保存
    }

    // 難易度を設定
    public void SetDifficult(int difficult)
    {
        saveData.difficult = Mathf.Clamp(difficult, 0, 2); // 0~2の範囲に制限
        SaveGameData(); // 更新後に保存
    }

    // チートモードを設定
    public void SetCheatMode(bool isCheatMode)
    {
        saveData.cheatMode = isCheatMode;
        SaveGameData(); // 更新後に保存
    }

    // セーブデータを取得 (必要に応じて外部からアクセス可能)
    public SaveData GetSaveData()
    {
        return saveData;
    }
}
