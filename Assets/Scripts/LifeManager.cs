using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public int lifeCount = 10;
    public TextMeshProUGUI lifeText;
    public CheckRation checkRation;

    private void Start()
    {
        try
        {
            // SaveManager���烉�C�t���擾
            if (SaveManager.Instance != null)
            {
                lifeCount = SaveManager.Instance.GetLifeCount();
                bool cheat = SaveManager.Instance.GetCheatMode();
                if (cheat)
                {
                    lifeCount = 50; // �`�[�g���[�h���L���ȏꍇ�A���C�t��50�ɐݒ�
                }
            }
            else
            {
                Debug.LogWarning("SaveManager��������܂���B�f�t�H���g�̃��C�t�����g�p���܂��B");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���C�t�̎擾���ɃG���[���������܂���: {ex.Message}");
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
                Debug.LogWarning("SaveManager��������܂���B���C�t�����Z�[�u�ł��܂���ł����B");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���C�t�̃Z�[�u���ɃG���[���������܂���: {ex.Message}");
        }
        CheckGameOver(lifeCount);
        return lifeCount;
    }

    public void UpdateLifeText()
    {
        lifeText.text = lifeCount.ToString();
    }

    public void CheckGameOver(int lifeCount)
    {
        if (lifeCount <= 0)
        {
            Debug.Log("�Q�[���I�[�o�[");
            // �Q�[���I�[�o�[�����������ɒǉ�
            checkRation.GameOver(); // CheckRation�N���X��GameOver���\�b�h���Ăяo��
        }
    }
}
