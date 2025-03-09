using UnityEngine;

public class BoundaryChecker : MonoBehaviour
{
    public Transform player;
    private TextureBoundaryDetector boundaryDetector;
    private SpriteRenderer spriteRenderer;
    public  bool isBoundary;
    void Start()
    {
        boundaryDetector = FindObjectOfType<TextureBoundaryDetector>();

        if (boundaryDetector == null)
        {
            Debug.LogError("[BoundaryChecker] TextureBoundaryDetector がシーン内に見つかりません。");
            return;
        }
        Debug.Log("[BoundaryChecker] TextureBoundaryDetector を取得しました: " + boundaryDetector.gameObject.name);

        spriteRenderer = boundaryDetector.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("[BoundaryChecker] SpriteRenderer が見つからないか、スプライトが設定されていません。");
            return;
        }
        Debug.Log("[BoundaryChecker] スプライトが設定されています: " + spriteRenderer.sprite.name);

        // プレイヤーの初期位置を境界内に設定
        SetPlayerInsideBoundary();
    }

    void Update()
    {
        if (boundaryDetector == null || spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        Vector2Int pixelPos = WorldToPixel(player.position);
        //Debug.Log("[BoundaryChecker] ワールド座標: " + player.position + " → ピクセル座標: " + pixelPos);

        if (pixelPos.x < 0 || pixelPos.x >= boundaryDetector.sourceTexture.width ||
            pixelPos.y < 0 || pixelPos.y >= boundaryDetector.sourceTexture.height)
        {
            Debug.LogWarning("[BoundaryChecker] プレイヤーのピクセル座標がテクスチャ範囲外: " + pixelPos);
            return;
        }

        if (boundaryDetector.IsOnBoundary(pixelPos))
        {
            //Debug.Log("[BoundaryChecker] プレイヤーは境界部分にいる！ ピクセル座標: " + pixelPos);
            isBoundary = true;

        }
        else
        {
            //Debug.Log("[BoundaryChecker] プレイヤーは境界部分にいない。");
            isBoundary = false;
        }
    }

    void SetPlayerInsideBoundary()
    {
        Vector2 textureCenter = new Vector2(boundaryDetector.sourceTexture.width / 2, boundaryDetector.sourceTexture.height / 2);
        Vector2Int pixelPos = new Vector2Int((int)textureCenter.x, (int)textureCenter.y);

        // 境界内になるまで調整
        while (!boundaryDetector.IsOnBoundary(pixelPos))
        {
            pixelPos.x = Mathf.Clamp(pixelPos.x - 1, 0, boundaryDetector.sourceTexture.width - 1);
            pixelPos.y = Mathf.Clamp(pixelPos.y - 1, 0, boundaryDetector.sourceTexture.height - 1);

            // 万が一、完全に外に出た場合は強制終了
            if (pixelPos.x == 0 && pixelPos.y == 0)
            {
                Debug.LogError("[BoundaryChecker] 境界が見つからないため、初期位置を設定できません。");
                return;
            }
        }

        // ワールド座標に変換してプレイヤーを移動
        player.position = PixelToWorld(pixelPos);
        Debug.Log("[BoundaryChecker] プレイヤーの初期位置を境界内に設定しました: " + player.position);
    }

    public Vector2Int WorldToPixel(Vector3 worldPos)
    {
        Vector3 localPos = boundaryDetector.transform.InverseTransformPoint(worldPos);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        if (spriteSize.x == 0 || spriteSize.y == 0)
        {
            Debug.LogError("[BoundaryChecker] スプライトのサイズが無効です。");
            return Vector2Int.zero;
        }

        int px = Mathf.RoundToInt((localPos.x + spriteSize.x / 2) / spriteSize.x * boundaryDetector.sourceTexture.width);
        int py = Mathf.RoundToInt((localPos.y + spriteSize.y / 2) / spriteSize.y * boundaryDetector.sourceTexture.height);
        return new Vector2Int(px, py);
    }

    Vector3 PixelToWorld(Vector2Int pixelPos)
    {
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float x = ((float)pixelPos.x / boundaryDetector.sourceTexture.width) * spriteSize.x - spriteSize.x / 2;
        float y = ((float)pixelPos.y / boundaryDetector.sourceTexture.height) * spriteSize.y - spriteSize.y / 2;

        return boundaryDetector.transform.TransformPoint(new Vector3(x, y, 0));
    }

    // プレイヤーに最も近い境界の点を探してその位置にワープさせる
    public void WarpToClosestBoundary()
    {
        if (boundaryDetector == null) return;

        Vector2 closestPoint = FindClosestBoundaryPoint(player.position);
        if (closestPoint != Vector2.zero)
        {
            player.position = closestPoint;
            Debug.Log("[BoundaryChecker] プレイヤーを最寄りの境界にワープさせました: " + closestPoint);
        }
        else
        {
            Debug.LogWarning("[BoundaryChecker] 境界が見つかりませんでした。");
        }
    }

    // プレイヤーの縦横方向の直線範囲内で最も近い境界の点を探す
    Vector2 FindClosestBoundaryPoint(Vector3 playerPosition)
    {
        float closestDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;

        // プレイヤーの位置をピクセル座標に変換
        Vector2Int playerPixelPos = WorldToPixel(playerPosition);
        int width = boundaryDetector.sourceTexture.width;
        int height = boundaryDetector.sourceTexture.height;

        // 縦横方向に範囲を制限して探索
        for (int y = 0; y < height; y++)
        {
            if (boundaryDetector.IsOnBoundary(new Vector2Int(playerPixelPos.x, y)))
            {
                Vector3 worldPos = PixelToWorld(new Vector2Int(playerPixelPos.x, y));
                float distance = Vector3.Distance(playerPosition, worldPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = worldPos;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            if (boundaryDetector.IsOnBoundary(new Vector2Int(x, playerPixelPos.y)))
            {
                Vector3 worldPos = PixelToWorld(new Vector2Int(x, playerPixelPos.y));
                float distance = Vector3.Distance(playerPosition, worldPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = worldPos;
                }
            }
        }

        return closestPoint;
    }
}
