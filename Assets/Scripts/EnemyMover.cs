using Unity.VisualScripting;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2.0f;
    public float cooldownDuration = 2.0f; // クールダウンの長さ（秒）
    public TextureBoundaryDetector textureBoundaryDetector;
    public LifeManager lifeManager;
    public PlayerMovementpix playerMovementpix;
    public float turnChance = 0.1f; // 方向転換の確率（10%）

    private Vector2 direction;
    private bool enemyOnLineCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        // 初期方向：45度（右上）
        float angle = 45f * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 nextPosition = currentPosition + direction * speed * Time.deltaTime;

        // クールダウン処理
        if (!enemyOnLineCooldown)
        {
            if (playerMovementpix.IsEnemyOnLine(nextPosition))
            {
                enemyOnLineCooldown = true;
                cooldownTimer = cooldownDuration;
                Debug.Log("Enemy touched line! Starting cooldown.");
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                enemyOnLineCooldown = false;
                Debug.Log("Cooldown ended.");
            }
        }

        // 通行可能なら移動
        if (!textureBoundaryDetector.IsInTransparentArea(nextPosition) && IsWithinScreenBounds(nextPosition))
        {
            transform.position = nextPosition;
        }
        else
        {
            // たまに反対方向に転換（ランダムで確率に基づいて）
            if (Random.Range(0f, 1f) < turnChance)
            {
                // 90度時計回りに回転
                direction = new Vector2(direction.y, -direction.x);
                Debug.Log("ランダム");
            }
            else
            {
                // 90度時計回りに回転
                direction = new Vector2(-direction.y, direction.x);
            }

        }
    }

    // 画面内にいるかをチェック（カメラビュー基準）
    private bool IsWithinScreenBounds(Vector2 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 &&
               screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    public void CheckArea()
    {
        //透明エリアにいるか
        if (textureBoundaryDetector.IsInTransparentArea(new Vector2(transform.position.x, transform.position.y)))
        {
            Debug.Log("やられたー");
            lifeManager.AddLife(1); // ライフ増やす
            Destroy(this.gameObject);
        }
    }
}
