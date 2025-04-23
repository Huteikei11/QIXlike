using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI����ɕK�v

public class TitleManager : MonoBehaviour
{
    public int titleMode; // �^�C�g�����=0, �X�e�[�W�Z���N�g=1, CG=2, �R���t�B�O=3
    public int chara; // �L�����N�^�[�̑I����Ԃ�ێ�����ϐ�

    // �e���[�h�ɑΉ�����I�u�W�F�N�g�̃��X�g
    public List<GameObject> titleScreenObjects; // �^�C�g����ʗp�I�u�W�F�N�g
    public List<GameObject> stageSelectObjects; // �X�e�[�W�Z���N�g��ʗp�I�u�W�F�N�g
    public List<GameObject> cgObjects; // CG��ʗp�I�u�W�F�N�g
    public List<GameObject> configObjects; // �R���t�B�O��ʗp�I�u�W�F�N�g
    public List<GameObject> buttonObjects; // ��ʑJ�ڗp�{�^��

    // �L�����N�^�[�I���{�^���̉摜���X�g
    public List<Image> charaImages; // �L�����N�^�[�I���{�^����Image�R���|�[�l���g
    public Image modebuttonImage; // �R���t�B�O�{�^����Image�R���|�[�l���g

    // �ύX����摜��Inspector����ݒ�
    public Sprite confirmSpriteMode1; // ���[�h1�p�̉摜
    public Sprite confirmSpriteMode2; // ���[�h2�p�̉摜

    // Start is called before the first frame update
    void Start()
    {
        titleMode = 0; // ������Ԃ̓^�C�g�����
        ChangeView(titleMode); // ������ʂ�ݒ�
    }

    // Update is called once per frame
    void Update()
    {
        if (titleMode == 0 && Input.GetKeyDown(KeyCode.Return)) // �^�C�g����ʂ�Enter�L�[�������ꂽ�ꍇ
        {
            SelectMode(1); // �X�e�[�W�Z���N�g��ʂɈړ�
        }
    }
    public void ConfilmButton() // Game���[�h�̂Ƃ�
    {
        if(titleMode == 1) // �X�e�[�W�Z���N�g���
        {
            // �L�����ԍ����擾���āA�Q�[�����J�n����
            GameStart(); // �Q�[�����J�n���郁�\�b�h���Ăяo��
        }
        else if (titleMode == 2) // CG���
        {
            // Save���炻�̃L�������N���A���Ă��邩�m�F
        }
    }

    public void ModeButton()//Game/CG�{�^��
    {
        if (titleMode == 1)
        {
            SelectMode(2);// CG���[�h�Ɉړ�
            modebuttonImage.sprite = confirmSpriteMode2; // ���[�h1�p�̉摜�ɕύX
        }
        else if (titleMode == 2) // CG���[�h
        {
            SelectMode(1); // �Q�[�����[�h�Ɉړ� 
            modebuttonImage.sprite = confirmSpriteMode1; // ���[�h2�p�̉摜�ɕύX
        }
    }

    public void SelectMode(int mode)//�R���t�B�O�E�^�C�g���͒��ړǂݍ���
    {
        titleMode = mode;
        ChangeView(mode); // ���[�h�ɉ����ĉ�ʂ�؂�ւ���
    }

    private void ChangeView(int mode) // ��ʂ����ۂɐ؂�ւ���
    {
        // ���ׂẴI�u�W�F�N�g���\���ɂ���
        SetActiveObjects(titleScreenObjects, false);
        SetActiveObjects(stageSelectObjects, false);
        SetActiveObjects(cgObjects, false);
        SetActiveObjects(configObjects, false);

        // �w�肳�ꂽ���[�h�̃I�u�W�F�N�g��\������
        switch (mode)
        {
            case 0: // �^�C�g�����
                SetActiveObjects(titleScreenObjects, true);
                SetActiveObjects(buttonObjects, false);
                break;
            case 1: // �X�e�[�W�Z���N�g
                SetActiveObjects(stageSelectObjects, true);
                SetActiveObjects(buttonObjects, true);
                break;
            case 2: // CG���[�h
                SetActiveObjects(cgObjects, true);
                SetActiveObjects(buttonObjects, true);
                break;
            case 3: // �R���t�B�O
                SetActiveObjects(configObjects, true);
                SetActiveObjects(buttonObjects, false);
                break;
            default:
                Debug.LogError("�����ȃ��[�h���w�肳��܂����B");
                break;
        }
    }

    private void SetActiveObjects(List<GameObject> objects, bool isActive)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive); // �I�u�W�F�N�g�̗L��/������؂�ւ���
            }
        }
    }

    public void SetChara(int charaIndex)
    {
        chara = charaIndex; // �L�����N�^�[�̑I����Ԃ��X�V
        Debug.Log("�L�����N�^�[���I������܂���: " + chara);

        // SaveManager �ɃL�����N�^�[����ۑ�
        SaveManager.Instance.SetCharacter(chara);

        UpdateCharaImages(); // �L�����N�^�[�摜�̓����x���X�V
    }

    private void UpdateCharaImages()//�I�������L�����̓����x��ύX����
    {
        for (int i = 0; i < charaImages.Count; i++)
        {
            if (charaImages[i] != null)
            {
                var color = charaImages[i].color;
                if (i == chara)
                {
                    color.a = 0.7f; // �I�����ꂽ�L�����N�^�[�̓����x���ő�ɂ���
                }
                else
                {
                    color.a = 1f; // ��I���L�����N�^�[�̓����x��������
                }
                charaImages[i].color = color; // �����x��K�p
            }
        }
    }

    private void GameStart()
    {
        try
        {
            // SaveManager��level��life��������
            int level = 0;
            int life = 10;

            SaveManager.Instance.SetLevel(level);
            SaveManager.Instance.SetLifeCount(life);

            Debug.Log($"�Q�[�����J�n���܂��B�I�����ꂽ�L�����N�^�[: {chara}, ���x��: {level}, ���C�t: {life}");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"SaveManager����f�[�^���擾�ł��܂���ł���: {ex.Message}");
        }
        // �V�[�������[�h
        SceneManager.LoadScene("Main");
    }

}
