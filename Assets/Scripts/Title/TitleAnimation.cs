using UnityEngine;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{
    public float fadeDuration = 1f; // フェードイン・アウトの時間
    public float moveDistance = 1f; // 上下移動の距離
    public float moveDuration = 2f; // 上下移動の時間
    public bool isLogo; // ロゴかどうかを判定

    private SpriteRenderer spriteRenderer; // フェード用
    private Transform spriteTransform; // 移動用

    private void Awake()
    {
        // 必要なコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteTransform = GetComponent<Transform>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRendererがアタッチされていません。");
        }

        if (spriteTransform == null)
        {
            Debug.LogError("Transformがアタッチされていません。");
        }
    }

    private void Start()
    {
        if (isLogo)
        {
            // ロゴのアニメーションを開始
            StartMoveAnimation();
        }
        else
        {
            // 文字のアニメーションを開始
            StartFadeAnimation();
        }
    }

    private void StartFadeAnimation()
    {
        if (spriteRenderer != null)
        {
            // フェードイン・アウトをループ
            spriteRenderer.DOFade(0f, fadeDuration)
                .SetLoops(-1, LoopType.Yoyo) // 無限ループでYoyo（行ったり来たり）
                .SetEase(Ease.InOutSine); // 滑らかなイージング
        }
    }

    private void StartMoveAnimation()
    {
        if (spriteTransform != null)
        {
            // 上下移動をループ
            spriteTransform.DOMoveY(spriteTransform.position.y + moveDistance, moveDuration)
                .SetLoops(-1, LoopType.Yoyo) // 無限ループでYoyo（行ったり来たり）
                .SetEase(Ease.InOutSine); // 滑らかなイージング
        }
    }
}
