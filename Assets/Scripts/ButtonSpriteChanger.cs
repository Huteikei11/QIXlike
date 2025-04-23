using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChanger : MonoBehaviour
{
    [SerializeField] private int chara; // Inspector�Őݒ肷��L�����N�^�[ID
    [SerializeField] private Sprite spriteTrue; // �X�e�[�W�N���A����Sprite
    [SerializeField] private Sprite spriteFalse; // �X�e�[�W���N���A����Sprite

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

        bool isStageClear = SaveManager.Instance.GetStageClear(chara);
        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.sprite = isStageClear ? spriteFalse : spriteTrue;
            Debug.Log($"Button��Sprite���X�V���܂���: {isStageClear}");
        }
        else
        {
            Debug.LogError("Button��Image�R���|�[�l���g���A�^�b�`����Ă��܂���B");
        }
    }
}
