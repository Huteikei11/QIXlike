using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterSpriteSet
{
    public string characterName; // キャラクター名 (任意)
    public Sprite[] sprites; // キャラクターごとのスプライト配列
}

public class CharacterSpriteManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // スプライトを表示するSpriteRenderer
    [SerializeField] private CharacterSpriteSet[] characterSpriteSets; // キャラクターごとのスプライトセット

    public int currentCharacterIndex = -1; // 現在のキャラクターインデックス
    public int currentSpriteIndex = 0; // 現在のスプライトインデックス

    private void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRendererが設定されていません。");
            return;
        }

        if (characterSpriteSets == null || characterSpriteSets.Length == 0)
        {
            Debug.LogError("キャラクタースプライトセットが設定されていません。");
            return;
        }

        LoadCharacterAndSpriteIndex();
        UpdateSprite();
    }

    private void LoadCharacterAndSpriteIndex()
    {
        // SaveManagerからキャラクターインデックスを取得
        currentCharacterIndex = SaveManager.Instance.GetCharacter();

        // キャラクターインデックスが無効な場合はデフォルト値を設定
        if (currentCharacterIndex < 0 || currentCharacterIndex >= characterSpriteSets.Length)
        {
            Debug.LogWarning("無効なキャラクターインデックスです。デフォルト値を使用します。");
            currentCharacterIndex = 0;
        }

        // 必要に応じてスプライトインデックスをロード (ここでは初期値を0に設定)
        currentSpriteIndex = 0;
    }

    private void UpdateSprite()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            if (currentSpriteIndex >= 0 && currentSpriteIndex < spriteSet.sprites.Length)
            {
                spriteRenderer.sprite = spriteSet.sprites[currentSpriteIndex];
                Debug.Log($"キャラクター {currentCharacterIndex} のスプライト {currentSpriteIndex} を表示しました。");
            }
            else
            {
                Debug.LogWarning("無効なスプライトインデックスです。デフォルトのスプライトを使用します。");
                spriteRenderer.sprite = null; // 必要に応じてデフォルトスプライトを設定
            }
        }
        else
        {
            Debug.LogError("無効なキャラクターインデックスです。");
        }
    }

    public void SetSpriteIndex(int spriteIndex)
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            // スプライトインデックスをスプライト数の範囲内に制限
            if (spriteIndex >= 0 && spriteIndex < spriteSet.sprites.Length)
            {
                currentSpriteIndex = spriteIndex;
                UpdateSprite();
                Debug.Log($"スプライトインデックスを {spriteIndex} に変更しました。");
            }
            else
            {
                Debug.LogError($"無効なスプライトインデックスです: {spriteIndex} (最大値: {spriteSet.sprites.Length - 1})");
            }
        }
        else
        {
            Debug.LogError("無効なキャラクターインデックスです。");
        }
    }

    public void SetCharacterIndex(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterSpriteSets.Length)
        {
            currentCharacterIndex = characterIndex;
            currentSpriteIndex = 0; // キャラクター変更時にスプライトインデックスをリセット
            UpdateSprite();
            Debug.Log($"キャラクターインデックスを {characterIndex} に変更しました。");
        }
        else
        {
            Debug.LogError("無効なキャラクターインデックスです: " + characterIndex);
        }
    }

    // スプライトインデックスを+1する
    public void IncrementSpriteIndex()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            if (currentSpriteIndex + 1 < spriteSet.sprites.Length)
            {
                currentSpriteIndex++;
                UpdateSprite();
                Debug.Log($"スプライトインデックスを+1しました: {currentSpriteIndex}");
            }
            else
            {
                Debug.LogWarning("スプライトインデックスはこれ以上増やせません。");
            }
        }
        else
        {
            Debug.LogError("無効なキャラクターインデックスです。");
        }
    }

    // スプライトインデックスを-1する
    public void DecrementSpriteIndex()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            if (currentSpriteIndex - 1 >= 0)
            {
                currentSpriteIndex--;
                UpdateSprite();
                Debug.Log($"スプライトインデックスを-1しました: {currentSpriteIndex}");
            }
            else
            {
                Debug.LogWarning("スプライトインデックスはこれ以上減らせません。");
            }
        }
        else
        {
            Debug.LogError("無効なキャラクターインデックスです。");
        }
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
