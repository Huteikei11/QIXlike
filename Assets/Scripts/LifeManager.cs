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
            // SaveManager���烉�C�t���擾
            if (SaveManager.Instance != null)
            {
                lifeCount = SaveManager.Instance.GetLifeCount();
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

        return lifeCount;
    }

    public void UpdateLifeText()
    {
        lifeText.text = lifeCount.ToString();
    }
}
