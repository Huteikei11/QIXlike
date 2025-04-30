using UnityEngine;
using UnityEngine.UI;

public class AdvancedButtonSpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite spriteTrue; // stage[6]がtrueの場合のSprite
    [SerializeField] private Sprite spriteFalse; // stage[0]からstage[5]がすべてtrueの場合のSprite
    [SerializeField] private Sprite unLockSprite; // 条件を満たさない場合のSprite

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

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("ButtonにImageコンポーネントがアタッチされていません。");
            return;
        }

        // stage[0]からstage[5]がすべてtrueかどうかを判定
        bool allStagesClear = true;
        for (int i = 0; i <= 5; i++)
        {
            if (!SaveManager.Instance.GetStageClear(i))
            {
                allStagesClear = false;
                break;
            }
        }

        // stage[6]の状態を取得
        bool isStage6Clear = SaveManager.Instance.GetStageClear(6);

        // 条件に応じてスプライトを変更
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

        Debug.Log($"ButtonのSpriteを更新しました: stage[6]={isStage6Clear}, allStagesClear={allStagesClear}");
    }
}
