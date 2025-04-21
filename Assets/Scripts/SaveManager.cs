using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;

    private const string SaveKey = "GameSaveData"; // �Z�[�u�f�[�^�̃L�[

    [System.Serializable]
    public class SaveData
    {
        public bool[] stage = new bool[7]; // �X�e�[�W�N���A��� (7�X�e�[�W��)
        public int volume = 5; // ���� (0~9)
        public bool mute = false; // �~���[�g���
        public int difficult = 0; // ��Փx (0: Easy, 1: Normal, 2: Hard)
        public bool cheatMode = false; // �`�[�g���[�h
    }

    private SaveData saveData = new SaveData();

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("SaveManager���V�[���ɑ��݂��܂���B");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �V�[���ԂŃI�u�W�F�N�g��j�����Ȃ�
        }
        else
        {
            Destroy(gameObject); // �d������C���X�^���X��j��
        }
    }

    public void Start()
    {
        Debug.Log("�Z�[�u�f�[�^�����[�h���܂��B");
        LoadData();
    }

    public void LoadData()
    {
        try
        {
            if (ES3.KeyExists(SaveKey)) // �Z�[�u�f�[�^�����݂��邩�m�F
            {
                saveData = ES3.Load<SaveData>(SaveKey);
                Debug.Log("�Z�[�u�f�[�^�����[�h���܂����B");
            }
            else
            {
                Debug.LogWarning("�Z�[�u�f�[�^�����݂��܂���B�V�����f�[�^���쐬���܂��B");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("�Z�[�u�f�[�^�̃��[�h���ɃG���[���������܂���: " + ex.Message);
        }
    }

    public void SaveGameData()
    {
        ES3.Save(SaveKey, saveData);
        Debug.Log("�Z�[�u�f�[�^��ۑ����܂����B");
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
            Debug.LogError("�����ȃX�e�[�W�C���f�b�N�X�ł�: " + stageIndex);
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
