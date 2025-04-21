using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;

    private const string SaveKey = "GameSaveData"; // セーブデータのキー

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

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("SaveManagerがシーンに存在しません。");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン間でオブジェクトを破棄しない
        }
        else
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
        }
    }

    public void Start()
    {
        Debug.Log("セーブデータをロードします。");
        LoadData();
    }

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

    public void SaveGameData()
    {
        ES3.Save(SaveKey, saveData);
        Debug.Log("セーブデータを保存しました。");
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void SetStageClear(int stageIndex, bool isClear)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            saveData.stage[stageIndex] = isClear;
            SaveGameData();
        }
        else
        {
            Debug.LogError("無効なステージインデックスです: " + stageIndex);
        }
    }

    public void SetVolume(int volume)
    {
        saveData.volume = Mathf.Clamp(volume, 0, 9);
        SaveGameData();
    }

    public void SetMute(bool isMute)
    {
        saveData.mute = isMute;
        SaveGameData();
    }

    public void SetDifficult(int difficult)
    {
        saveData.difficult = Mathf.Clamp(difficult, 0, 2);
        SaveGameData();
    }

    public void SetCheatMode(bool isCheatMode)
    {
        saveData.cheatMode = isCheatMode;
        SaveGameData();
    }
}
