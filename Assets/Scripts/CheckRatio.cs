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
    public float clearRatio = 0.1f; // �N���A�����̔䗦
    public float CalculateTransparencyRatio(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture2D��null�ł��B");
            return 0f;
        }

        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int transparentPixels = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a == 0f) // �A���t�@�l��0�̏ꍇ�A�����Ƃ݂Ȃ�
            {
                transparentPixels++;
            }
        }
        ratio = (float)transparentPixels / totalPixels;
        DisplayRatioAsPercent();

        CheckClear();//�N���A����
        return ratio;
    }

    public void DisplayRatioAsPercent()
    {
        if (areaRatio == null)
        {
            Debug.LogWarning("TextMeshProUGUI���ݒ肳��Ă��܂���B");
            return;
        }

        // ratio���p�[�Z���g�\�L�ɕϊ�
        float percent = ratio * 100f;
        float clearPercent = clearRatio * 100f; // �N���A�����̃p�[�Z���g

        // TextMeshProUGUI�ɕ\��
        areaRatio.text = $"{percent:F1}/{clearPercent:F0}"; // �����_�ȉ�2���܂ŕ\��
    }

    private void CheckClear()
    {
        if (ratio > clearRatio)
        {
            Debug.Log("�N���A�����𖞂����܂����I");
            //�N���A���o

            // �N���A�����������ɒǉ�
            try
            {
                // SaveManager����level��life���擾���ď�����
                int level = SaveManager.Instance.GetLevel();
                int life = SaveManager.Instance.GetLifeCount();

                SaveManager.Instance.SetLevel(level+1);
                SaveManager.Instance.SetLifeCount(life);

                Debug.Log($"�Q�[�����J�n���܂��B ���x��: {level}, ���C�t: {life}");
            }
            catch (System.Exception ex)
            {
                Debug.Log($"SaveManager����f�[�^���擾�ł��܂���ł���: {ex.Message}");
            }
            // �V�[�������[�h
            SceneManager.LoadScene("Main");
        }
    }
}
