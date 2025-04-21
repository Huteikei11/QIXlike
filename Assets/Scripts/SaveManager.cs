using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveKey = "GameSaveData"; // �Z�[�u�f�[�^�̃L�[

    // �Z�[�u�f�[�^�\����
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

    public void Start()
    {
        // ���������ɃZ�[�u�f�[�^�����[�h
        Debug.Log("�Z�[�u�f�[�^�����[�h���܂��B");
        LoadData();
    }
    // �Z�[�u�f�[�^�����[�h
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

    // �Z�[�u�f�[�^��ۑ�
    public void SaveGameData()
    {
        ES3.Save(SaveKey, saveData);
        Debug.Log("�Z�[�u�f�[�^��ۑ����܂����B");
    }

    // �X�e�[�W�N���A�����X�V
    public void SetStageClear(int stageIndex, bool isClear)
    {
        if (stageIndex >= 0 && stageIndex < saveData.stage.Length)
        {
            saveData.stage[stageIndex] = isClear;
            SaveGameData(); // �X�V��ɕۑ�
        }
        else
        {
            Debug.LogError("�����ȃX�e�[�W�C���f�b�N�X�ł�: " + stageIndex);
        }
    }

    // ���ʂ�ݒ�
    public void SetVolume(int volume)
    {
        saveData.volume = Mathf.Clamp(volume, 0, 9); // 0~9�͈̔͂ɐ���
        SaveGameData(); // �X�V��ɕۑ�
    }

    // �~���[�g��Ԃ�ݒ�
    public void SetMute(bool isMute)
    {
        saveData.mute = isMute;
        SaveGameData(); // �X�V��ɕۑ�
    }

    // ��Փx��ݒ�
    public void SetDifficult(int difficult)
    {
        saveData.difficult = Mathf.Clamp(difficult, 0, 2); // 0~2�͈̔͂ɐ���
        SaveGameData(); // �X�V��ɕۑ�
    }

    // �`�[�g���[�h��ݒ�
    public void SetCheatMode(bool isCheatMode)
    {
        saveData.cheatMode = isCheatMode;
        SaveGameData(); // �X�V��ɕۑ�
    }

    // �Z�[�u�f�[�^���擾 (�K�v�ɉ����ĊO������A�N�Z�X�\)
    public SaveData GetSaveData()
    {
        return saveData;
    }
}
