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
        public int chara = -1; // �L�����N�^�[��� (�����l�͖��I�������� -1)
        public int level = 0; // �Q�[���̐i���x (�����l��0)
    }

    private SaveData saveData = new SaveData();

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("SaveManager���V�[���ɑ��݂��܂���B");
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

    // �X�e�[�W�N���A���
    public void SetStageClear(int stageIndex, bool isClear)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            saveData.stage[stageIndex] = isClear;
            SaveGameData();
            Debug.Log($"�X�e�[�W {stageIndex} �̃N���A��Ԃ�ݒ�: {isClear}");
        }
        else
        {
            Debug.LogError("�����ȃX�e�[�W�C���f�b�N�X�ł�: " + stageIndex);
        }
    }

    public bool GetStageClear(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            Debug.Log($"�X�e�[�W {stageIndex} �̃N���A��Ԃ��擾: {saveData.stage[stageIndex]}");
            return saveData.stage[stageIndex];
        }
        else
        {
            Debug.LogError("�����ȃX�e�[�W�C���f�b�N�X�ł�: " + stageIndex);
            return false;
        }
    }

    // ����
    public void SetVolume(int volume)
    {
        saveData.volume = Mathf.Clamp(volume, 0, 9);
        SaveGameData();
        Debug.Log($"���ʂ�ݒ�: {volume}");
    }

    public int GetVolume()
    {
        Debug.Log($"���ʂ��擾: {saveData.volume}");
        return saveData.volume;
    }

    // �~���[�g���
    public void SetMute(bool isMute)
    {
        saveData.mute = isMute;
        SaveGameData();
        Debug.Log($"�~���[�g��Ԃ�ݒ�: {isMute}");
    }

    public bool GetMute()
    {
        Debug.Log($"�~���[�g��Ԃ��擾: {saveData.mute}");
        return saveData.mute;
    }

    // ��Փx
    public void SetDifficult(int difficult)
    {
        saveData.difficult = Mathf.Clamp(difficult, 0, 2); // ��Փx��0~2�͈̔͂ɐ���
        SaveGameData();
        Debug.Log($"��Փx��ݒ�: {difficult}");
    }

    public int GetDifficult()
    {
        Debug.Log($"��Փx���擾: {saveData.difficult}");
        return saveData.difficult;
    }

    // �`�[�g���[�h
    public void SetCheatMode(bool isCheatMode)
    {
        saveData.cheatMode = isCheatMode;
        SaveGameData();
        Debug.Log($"�`�[�g���[�h��ݒ�: {isCheatMode}");
    }

    public bool GetCheatMode()
    {
        Debug.Log($"�`�[�g���[�h���擾: {saveData.cheatMode}");
        return saveData.cheatMode;
    }

    // �L�����N�^�[���
    public void SetCharacter(int characterIndex)
    {
        saveData.chara = characterIndex;
        SaveGameData();
        Debug.Log($"�L�����N�^�[��ݒ�: {characterIndex}");
    }

    public int GetCharacter()
    {
        Debug.Log($"�L�����N�^�[���擾: {saveData.chara}");
        return saveData.chara;
    }

    // ���x�����
    public void SetLevel(int level)
    {
        saveData.level = level;
        SaveGameData();
        Debug.Log($"���x����ݒ�: {level}");
    }

    public int GetLevel()
    {
        Debug.Log($"���x�����擾: {saveData.level}");
        return saveData.level;
    }
}
