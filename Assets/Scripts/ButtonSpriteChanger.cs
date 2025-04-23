using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChanger : MonoBehaviour
{
    [SerializeField] private int chara; // Inspectorで設定するキャラクターID
    [SerializeField] private Sprite spriteTrue; // ステージクリア時のSprite
    [SerializeField] private Sprite spriteFalse; // ステージ未クリア時のSprite

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Buttonコンポーネントが見つかりません。");
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
            Debug.LogError("SaveManagerのインスタンスが存在しません。");
            return;
        }

        bool isStageClear = SaveManager.Instance.GetStageClear(chara);
        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.sprite = isStageClear ? spriteFalse : spriteTrue;
            Debug.Log($"ButtonのSpriteを更新しました: {isStageClear}");
        }
        else
        {
            Debug.LogError("ButtonにImageコンポーネントがアタッチされていません。");
        }
    }
}
