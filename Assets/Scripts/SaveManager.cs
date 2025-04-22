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
        public int chara = -1; // キャラクター情報 (初期値は未選択を示す -1)
        public int level = 0; // ゲームの進捗度 (初期値は0)
    }

    private SaveData saveData = new SaveData();

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("SaveManagerがシーンに存在しません。");
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

    // ステージクリア情報
    public void SetStageClear(int stageIndex, bool isClear)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            saveData.stage[stageIndex] = isClear;
            SaveGameData();
            Debug.Log($"ステージ {stageIndex} のクリア状態を設定: {isClear}");
        }
        else
        {
            Debug.LogError("無効なステージインデックスです: " + stageIndex);
        }
    }

    public bool GetStageClear(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            Debug.Log($"ステージ {stageIndex} のクリア状態を取得: {saveData.stage[stageIndex]}");
            return saveData.stage[stageIndex];
        }
        else
        {
            Debug.LogError("無効なステージインデックスです: " + stageIndex);
            return false;
        }
    }

    // 音量
    public void SetVolume(int volume)
    {
        saveData.volume = Mathf.Clamp(volume, 0, 9);
        SaveGameData();
        Debug.Log($"音量を設定: {volume}");
    }

    public int GetVolume()
    {
        Debug.Log($"音量を取得: {saveData.volume}");
        return saveData.volume;
    }

    // ミュート状態
    public void SetMute(bool isMute)
    {
        saveData.mute = isMute;
        SaveGameData();
        Debug.Log($"ミュート状態を設定: {isMute}");
    }

    public bool GetMute()
    {
        Debug.Log($"ミュート状態を取得: {saveData.mute}");
        return saveData.mute;
    }

    // 難易度
    public void SetDifficult(int difficult)
    {
        saveData.difficult = Mathf.Clamp(difficult, 0, 2); // 難易度を0~2の範囲に制限
        SaveGameData();
        Debug.Log($"難易度を設定: {difficult}");
    }

    public int GetDifficult()
    {
        Debug.Log($"難易度を取得: {saveData.difficult}");
        return saveData.difficult;
    }

    // チートモード
    public void SetCheatMode(bool isCheatMode)
    {
        saveData.cheatMode = isCheatMode;
        SaveGameData();
        Debug.Log($"チートモードを設定: {isCheatMode}");
    }

    public bool GetCheatMode()
    {
        Debug.Log($"チートモードを取得: {saveData.cheatMode}");
        return saveData.cheatMode;
    }

    // キャラクター情報
    public void SetCharacter(int characterIndex)
    {
        saveData.chara = characterIndex;
        SaveGameData();
        Debug.Log($"キャラクターを設定: {characterIndex}");
    }

    public int GetCharacter()
    {
        Debug.Log($"キャラクターを取得: {saveData.chara}");
        return saveData.chara;
    }

    // レベル情報
    public void SetLevel(int level)
    {
        saveData.level = level;
        SaveGameData();
        Debug.Log($"レベルを設定: {level}");
    }

    public int GetLevel()
    {
        Debug.Log($"レベルを取得: {saveData.level}");
        return saveData.level;
    }
}
