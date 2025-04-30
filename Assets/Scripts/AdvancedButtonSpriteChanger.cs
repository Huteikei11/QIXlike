using UnityEngine;
using UnityEngine.UI;

public class AdvancedButtonSpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite spriteTrue; // stage[6]��true�̏ꍇ��Sprite
    [SerializeField] private Sprite spriteFalse; // stage[0]����stage[5]�����ׂ�true�̏ꍇ��Sprite
    [SerializeField] private Sprite unLockSprite; // �����𖞂����Ȃ��ꍇ��Sprite

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button�R���|�[�l���g��������܂���B");
            return;
        }
    }

    private void Update()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager�̃C���X�^���X�����݂��܂���B");
            return;
        }

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("Button��Image�R���|�[�l���g���A�^�b�`����Ă��܂���B");
            return;
        }

        // stage[0]����stage[5]�����ׂ�true���ǂ����𔻒�
        bool allStagesClear = true;
        for (int i = 0; i <= 5; i++)
        {
            if (!SaveManager.Instance.GetStageClear(i))
            {
                allStagesClear = false;
                break;
            }
        }

        // stage[6]�̏�Ԃ��擾
        bool isStage6Clear = SaveManager.Instance.GetStageClear(6);

        // �����ɉ����ăX�v���C�g��ύX
        if (isStage6Clear)
        {
            buttonImage.sprite = spriteTrue;
        }
        else if (allStagesClear)
        {
            buttonImage.sprite = spriteFalse;
        }
        else
        {
            buttonImage.sprite = unLockSprite;
        }

        Debug.Log($"Button��Sprite���X�V���܂���: stage[6]={isStage6Clear}, allStagesClear={allStagesClear}");
    }
}
