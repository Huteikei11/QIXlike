using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI操作に必要

public class TitleManager : MonoBehaviour
{
    public int titleMode; // タイトル画面=0, ステージセレクト=1, CG=2, コンフィグ=3
    public int chara; // キャラクターの選択状態を保持する変数

    // 各モードに対応するオブジェクトのリスト
    public List<GameObject> titleScreenObjects; // タイトル画面用オブジェクト
    public List<GameObject> stageSelectObjects; // ステージセレクト画面用オブジェクト
    public List<GameObject> cgObjects; // CG画面用オブジェクト
    public List<GameObject> configObjects; // コンフィグ画面用オブジェクト
    public List<GameObject> buttonObjects; // 画面遷移用ボタン

    // キャラクター選択ボタンの画像リスト
    public List<Image> charaImages; // キャラクター選択ボタンのImageコンポーネント
    public Image modebuttonImage; // コンフィグボタンのImageコンポーネント

    // 変更する画像をInspectorから設定
    public Sprite confirmSpriteMode1; // モード1用の画像
    public Sprite confirmSpriteMode2; // モード2用の画像

    // Start is called before the first frame update
    void Start()
    {
        titleMode = 0; // 初期状態はタイトル画面
        ChangeView(titleMode); // 初期画面を設定
    }

    // Update is called once per frame
    void Update()
    {
        if (titleMode == 0 && Input.GetKeyDown(KeyCode.Return)) // タイトル画面でEnterキーが押された場合
        {
            SelectMode(1); // ステージセレクト画面に移動
        }
    }
    public void ConfilmButton() // Gameモードのとき
    {
        if(titleMode == 1) // ステージセレクト画面
        {
            // キャラ番号を取得して、ゲームを開始する
            GameStart(); // ゲームを開始するメソッドを呼び出す
        }
        else if (titleMode == 2) // CG画面
        {
            // Saveからそのキャラをクリアしているか確認
        }
    }

    public void ModeButton()//Game/CGボタン
    {
        if (titleMode == 1)
        {
            SelectMode(2);// CGモードに移動
            modebuttonImage.sprite = confirmSpriteMode2; // モード1用の画像に変更
        }
        else if (titleMode == 2) // CGモード
        {
            SelectMode(1); // ゲームモードに移動 
            modebuttonImage.sprite = confirmSpriteMode1; // モード2用の画像に変更
        }
    }

    public void SelectMode(int mode)//コンフィグ・タイトルは直接読み込み
    {
        titleMode = mode;
        ChangeView(mode); // モードに応じて画面を切り替える
    }

    private void ChangeView(int mode) // 画面を実際に切り替える
    {
        // すべてのオブジェクトを非表示にする
        SetActiveObjects(titleScreenObjects, false);
        SetActiveObjects(stageSelectObjects, false);
        SetActiveObjects(cgObjects, false);
        SetActiveObjects(configObjects, false);

        // 指定されたモードのオブジェクトを表示する
        switch (mode)
        {
            case 0: // タイトル画面
                SetActiveObjects(titleScreenObjects, true);
                SetActiveObjects(buttonObjects, false);
                break;
            case 1: // ステージセレクト
                SetActiveObjects(stageSelectObjects, true);
                SetActiveObjects(buttonObjects, true);
                break;
            case 2: // CGモード
                SetActiveObjects(cgObjects, true);
                SetActiveObjects(buttonObjects, true);
                break;
            case 3: // コンフィグ
                SetActiveObjects(configObjects, true);
                SetActiveObjects(buttonObjects, false);
                break;
            default:
                Debug.LogError("無効なモードが指定されました。");
                break;
        }
    }

    private void SetActiveObjects(List<GameObject> objects, bool isActive)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive); // オブジェクトの有効/無効を切り替える
            }
        }
    }

    public void SetChara(int charaIndex)
    {
        chara = charaIndex; // キャラクターの選択状態を更新
        Debug.Log("キャラクターが選択されました: " + chara);

        // SaveManager にキャラクター情報を保存
        SaveManager.Instance.SetCharacter(chara);

        UpdateCharaImages(); // キャラクター画像の透明度を更新
    }

    private void UpdateCharaImages()//選択したキャラの透明度を変更する
    {
        for (int i = 0; i < charaImages.Count; i++)
        {
            if (charaImages[i] != null)
            {
                var color = charaImages[i].color;
                if (i == chara)
                {
                    color.a = 0.7f; // 選択されたキャラクターの透明度を最大にする
                }
                else
                {
                    color.a = 1f; // 非選択キャラクターの透明度を下げる
                }
                charaImages[i].color = color; // 透明度を適用
            }
        }
    }

    private void GameStart()
    {
        try
        {
            // SaveManagerのlevelとlifeを初期化
            int level = 0;
            int life = 10;

            SaveManager.Instance.SetLevel(level);
            SaveManager.Instance.SetLifeCount(life);

            Debug.Log($"ゲームを開始します。選択されたキャラクター: {chara}, レベル: {level}, ライフ: {life}");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"SaveManagerからデータを取得できませんでした: {ex.Message}");
        }
        // シーンをロード
        SceneManager.LoadScene("Main");
    }

}
