using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro���g�p���邽�߂̖��O���

public class StageNumView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText; // TextMeshPro�̎Q�Ƃ�ݒ�

    // Start is called before the first frame update
    void Start()
    {
        int levelCount = 1; // �f�t�H���g�l��1�ɐݒ�

        try
        {
            // SaveManager����level���擾
            levelCount = SaveManager.Instance.GetLevel();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"SaveManager����f�[�^���擾�ł��܂���ł���: {ex.Message}");
            return; // ��O�����������ꍇ�A�Q�[���J�n�������X�L�b�v
        }

        // TextMeshPro�ɒl��\��
        if (stageText != null)
        {
            stageText.text = $"{levelCount+1}";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI���ݒ肳��Ă��܂���B");
        }
    }
}
