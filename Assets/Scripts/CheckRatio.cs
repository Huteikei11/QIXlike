using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTweenを使用するための名前空間

public class CheckRation : MonoBehaviour
{
    public float ratio;
    public TextMeshProUGUI areaRatio;
    public TextMeshProUGUI stageClearText; // STAGE CLEAR用のTextMeshPro
    public TextMeshProUGUI gameClearText; // GAME CLEAR用のTextMeshPro
    public List<SpriteRenderer> spriteRenderersToFade; // フェードアウトさせるSpriteRendererのリスト
    public CanvasGroup transitionCanvasGroup; // トランジション用のCanvasGroup
    public float clearRatio = 0.6f; // クリア条件の比率
    private int level;
    private int chara;
    private int life;
    public bool Isfinal;

    public TextureBoundaryDetector textureBoundaryDetector;
    public TextMeshProUGUI gameoverText;

    public float CalculateTransparencyRatio(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture2Dがnullです。");
            return 0f;
        }

        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int transparentPixels = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a == 0f) // アルファ値が0の場合、透明とみなす
            {
                transparentPixels++;
            }
        }
        ratio = (float)transparentPixels / totalPixels;
        DisplayRatioAsPercent();

        CheckClear(); // クリア判定
        return ratio;
    }

    public void DisplayRatioAsPercent()
    {
        if (areaRatio == null)
        {
            Debug.LogWarning("TextMeshProUGUIが設定されていません。");
            return;
        }

        // ratioをパーセント表記に変換
        float percent = ratio * 100f;
        float clearPercent = clearRatio * 100f; // クリア条件のパーセント

        // TextMeshProUGUIに表示
        areaRatio.text = $"{percent:F1}/{clearPercent:F0}"; // 小数点以下2桁まで表示
    }

    private void CheckClear()
    {
        if (ratio >= clearRatio)
        {
            Debug.Log("クリア条件を満たしました！");
            CheckLastStage();
        }
    }

    private void CheckLastStage()
    {
        try
        {
            // SaveManagerからlevelとlifeを取得
            level = SaveManager.Instance.GetLevel();
            chara = SaveManager.Instance.GetCharacter();

            int pixcount =GetTexturesCountForCurrentCharacter(chara); // キャラクターのテクスチャ数を取得

            Isfinal = (level >= pixcount - 2);//クリア処理
            // 枚数,ステージ数の-1,0数えの-1
            StartCoroutine(StageClearSequence());// ステージクリアシーケンスを開始
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveManagerからデータを取得できませんでした: {ex.Message}");
            return; // 例外が発生した場合、ゲーム開始処理をスキップ
        }
    }

    public int GetTexturesCountForCurrentCharacter(int chara)
    {
        if (textureBoundaryDetector == null)
        {
            Debug.LogError("TextureBoundaryDetectorが設定されていません。");
            return 0;
        }

        if (textureBoundaryDetector.characterTextures == null || textureBoundaryDetector.characterTextures.Count <= chara)
        {
            Debug.LogError($"CharacterTexturesが設定されていないか、charaインデックス({chara})が範囲外です。");
            return 0;
        }

        // charaインデックスに対応するtexturesの数を取得
        int texturesCount = textureBoundaryDetector.characterTextures[chara].textures.Count;
        Debug.Log($"キャラクター {chara} のtexturesの数: {texturesCount}");
        return texturesCount;
    }

    private IEnumerator StageClearSequence()
    {
        // STAGE CLEARの文字を表示
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.DOFade(1f, 0.5f); // フェードイン
        }
        else
        {
            Debug.LogWarning("stageClearTextが設定されていません。");
        }

        // 2秒間待機
        yield return new WaitForSeconds(1f);
        // PlayerとEnemyのオブジェクトを非表示
        HidePlayerAndEnemyObjects();

        // STAGE CLEARの文字を徐々に透明にする
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.DOFade(0f, 0.5f); // フェードイン
        }
        // SpriteRendererを徐々に透明にする
        foreach (var spriteRenderer in spriteRenderersToFade)
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FadeOutSprite(spriteRenderer));
            }
            else
            {
                Debug.LogWarning("spriteRenderersToFadeの中にnullが含まれています。");
            }
        }

        // 待機
        yield return new WaitForSeconds(1f);

        // キー入力待ち
        yield return new WaitUntil(() => Input.anyKeyDown);

        if (Isfinal)//最終ステージ?
        {
            // トランジション開始
            yield return StartCoroutine(TransitionToLastScene());
        }
        else
        {
            // トランジション開始
            yield return StartCoroutine(TransitionToNextScene());
        }
    }

    private IEnumerator FadeOutSprite(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null) yield break;

        float duration = 1f; // フェードアウトの時間
        float elapsed = 0f;

        Color originalColor = spriteRenderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
    }

    private IEnumerator TransitionToNextScene()//通常
    {
        if (transitionCanvasGroup != null)
        {
            // 画面を徐々に暗くする
             yield return transitionCanvasGroup.DOFade(1f, 1f).WaitForCompletion();

            // シーン遷移

            ChangeScene();

            // シーン遷移後、画面を徐々に明るくする
            yield return transitionCanvasGroup.DOFade(0f, 1f).WaitForCompletion();
         }
        else
        {
        Debug.LogWarning("transitionCanvasGroupが設定されていません。");
        }
    }

    private IEnumerator TransitionToLastScene()// 最終ステージ後
    {
        // 演出
        // GAME CLEARの文字を表示
        if (gameClearText != null)
        {
            gameClearText.gameObject.SetActive(true);
            gameClearText.DOFade(1f, 0.5f); // フェードイン
        }
        else
        {
            Debug.LogWarning("stageClearTextが設定されていません。");
        }

        // 待機
        yield return new WaitForSeconds(1f);

        // キー入力待ち
        yield return new WaitUntil(() => Input.anyKeyDown);

        if (transitionCanvasGroup != null)
        {
            // 画面を徐々に暗くする
            yield return transitionCanvasGroup.DOFade(1f, 1f).WaitForCompletion();

            // シーン遷移
            ToTitle();

            // シーン遷移後、画面を徐々に明るくする
            yield return transitionCanvasGroup.DOFade(0f, 1f).WaitForCompletion();
        }
        else
        {
            Debug.LogWarning("transitionCanvasGroupが設定されていません。");
        }
    }

    private void ChangeScene()// 次のステージ
    {
        try
        {
            // SaveManagerからlevelとlifeを取得して初期化
            level = SaveManager.Instance.GetLevel();
            life = SaveManager.Instance.GetLifeCount();

            SaveManager.Instance.SetLevel(level+1);
            SaveManager.Instance.SetLifeCount(life);

            Debug.Log($"ゲームを開始します レベル: {level}, ライフ: {life}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveManagerからデータを取得できませんでした: {ex.Message}");
            return; // 例外が発生した場合、ゲーム開始処理をスキップ
        }

        // シーンをロード
        SceneManager.LoadScene("Main");
    }

    private void ToTitle()
    {
        // クリアしたステージを更新
        try
        {
            // キャラに対応したステージをクリアにする
            SaveManager.Instance.SetStageClear(chara,true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveManagerからデータを取得できませんでした: {ex.Message}");
            return; // 例外が発生した場合、ゲーム開始処理をスキップ
        }

        // タイトルシーンに遷移
        SceneManager.LoadScene("Title");
        Debug.Log("タイトルシーンに遷移しました。");
    }

    private void HidePlayerAndEnemyObjects()
    {
        // Playerタグのオブジェクトを非表示
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.SetActive(false);
        }

        // Enemyタグのオブジェクトを非表示
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }

        Debug.Log("PlayerとEnemyのオブジェクトを非表示にしました。");
    }

    public void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // STAGE CLEARの文字を表示
        if (gameoverText != null)
        {
            gameoverText.gameObject.SetActive(true);
            gameoverText.DOFade(1f, 0.5f); // フェードイン
        }
        else
        {
            Debug.LogWarning("stageClearTextが設定されていません。");
        }

        // 2秒間待機
        yield return new WaitForSeconds(1f);
        // PlayerとEnemyのオブジェクトを非表示
        HidePlayerAndEnemyObjects();

        // 待機
        yield return new WaitForSeconds(1f);

        // キー入力待ち
        yield return new WaitUntil(() => Input.anyKeyDown);

        SceneManager.LoadScene("Title"); // タイトル
    }
}
