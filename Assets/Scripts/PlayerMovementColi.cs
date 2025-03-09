using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementColi : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private MaskController maskController;

    private bool isOutsideMyArea = false;
    private List<Vector2> pathPoints = new List<Vector2>();
    private Vector2 lastDirection;

    public float MaxY;
    public float MinY;
    public float MaxX;
    public float MinX;

    public LineRenderer lineRenderer;  // LineRenderer

    // 接触しているオブジェクトを管理するリスト
    private List<Collider2D> currentColliders = new List<Collider2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maskController = FindObjectOfType<MaskController>(); // MaskControllerを取得
        lineRenderer.positionCount = 0;  // 最初は線を描画しない

        if (maskController == null)
        {
            Debug.LogError("MaskController がシーン内に見つかりません！");
        }
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 && moveY != 0)
        {
            moveY = 0; // 斜め移動禁止
        }

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection != Vector2.zero && moveDirection != lastDirection)
        {
            RecordPoint();
            lastDirection = moveDirection;
        }
        // LineRendererで経路を描画
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));
    }

    void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;

        if (MaxX < transform.position.x)
        {
            transform.position = new Vector2(MaxX, transform.position.y);
            Debug.Log("右");
            StopPlayer();
        }
        if (MinX > transform.position.x)
        {
            Debug.Log("左");
            transform.position = new Vector2(MinX, transform.position.y);
            StopPlayer();
        }
        if (MaxY < transform.position.y)
        {
            Debug.Log("上");
            transform.position = new Vector2(transform.position.x, MaxY);
            StopPlayer();
        }
        if (MinY > transform.position.y)
        {
            Debug.Log("下");
            transform.position = new Vector2(transform.position.x, MinY);
            StopPlayer();
        }
    }
    void StopPlayer()
    {
        rb.velocity = new Vector2(0, 0);
    }

    void RecordPoint()
    {
        pathPoints.Add(transform.position);
    }

    private Vector3[] ConvertToVector3Array(List<Vector2> pathPoints)
    {
        pathPoints.Add(transform.position);
        Vector3[] positions = new Vector3[pathPoints.Count];
        for (int i = 0; i < pathPoints.Count; i++)
        {
            positions[i] = new Vector3(pathPoints[i].x, pathPoints[i].y, 0);  // z軸は0に設定
        }
        return positions;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter");
        Collider2D other = collision.collider;
        if (!currentColliders.Contains(other))
        {
            currentColliders.Add(other);
        }

        if (other.CompareTag("myarea"))
        {
            StopPlayer();
            if (isOutsideMyArea)
            {
                pathPoints.Add(transform.position); // 最終点
                GeneratePolygonCollider();
            }
            isOutsideMyArea = false;
            pathPoints.Clear();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Exit");
        Collider2D other = collision.collider;
        if (currentColliders.Contains(other))
        {
            currentColliders.Remove(other);
        }

        bool hasAreaTag = currentColliders.Any(collider => collider.CompareTag("myarea"));
        if (!hasAreaTag && other.CompareTag("myarea"))
        {
            isOutsideMyArea = true;
            RecordPoint(); // 最初の点を記録
        }
    }

    void GeneratePolygonCollider()
    {
        if (pathPoints.Count < 3)
        {
            Debug.LogWarning("多角形が成立しません。ランダムな方向で修正を試みます。");
            AdjustPathForPolygon();
        }
        if (pathPoints.Count % 2 == 1)//奇数なら点を足す
        {
            if (pathPoints.Count == 3)
            {
                AddPointToEvenNumber(pathPoints[0], pathPoints[1], pathPoints[2]);
            }
            else
            {
                Vector2 pointB = CalculateRightAnglePoint(pathPoints[0], pathPoints[pathPoints.Count - 1]);
                pathPoints.Add(pointB);
            }
        }

        GameObject newPoly = new GameObject("GeneratedPolygon");
        PolygonCollider2D polyCollider = newPoly.AddComponent<PolygonCollider2D>();

        polyCollider.points = pathPoints.ToArray();
        newPoly.tag = "myarea";

        Debug.Log($"生成された PolygonCollider2D の点の数: {polyCollider.points.Length}");
        foreach (Vector2 point in polyCollider.points)
        {
            Debug.Log($"点: {point}");
        }

        if (maskController != null)
        {
            maskController.ApplyMask(polyCollider);
        }
    }

    void AddPointToEvenNumber(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        // 直角三角形の直角がp2にあるので、直角を形成する2辺を基に残りの点を計算
        // p1とp3が直角三角形の辺で、p2が直角
        Vector2 p4 = p1 + p3 - p2;
        pathPoints.Add(p4);
    }

    // 点Aと点Cから直角三角形の直角の点Bを計算
    Vector2 CalculateRightAnglePoint(Vector2 A, Vector2 C)
    {
        // 直線ACのベクトルを求める
        Vector2 AC = C - A;

        // ACの方向ベクトルに垂直なベクトルを計算
        // 2Dベクトルの垂直ベクトルは (x, y) -> (-y, x) または (y, -x)
        Vector2 perpendicular = new Vector2(-AC.y, AC.x);

        // 点Cの象限を確認
        Vector2 pointB = Vector2.zero;
        if (C.x > 0 && C.y > 0)  // 第一象限
        {
            pointB = C + perpendicular;  // 右上
        }
        else if (C.x < 0 && C.y > 0)  // 第二象限
        {
            pointB = C + perpendicular;  // 左上
        }
        else if (C.x < 0 && C.y < 0)  // 第三象限
        {
            pointB = C - perpendicular;  // 左下
        }
        else if (C.x > 0 && C.y < 0)  // 第四象限
        {
            pointB = C - perpendicular;  // 右下
        }

        return pointB;
    }



    void AdjustPathForPolygon()
    {
        if (pathPoints.Count < 2) return;

        Vector2 lastPoint = pathPoints[pathPoints.Count - 1];
        Vector2 firstPoint = pathPoints[0];

        Vector2 midPoint = (lastPoint + firstPoint) / 2;
        pathPoints.Insert(1, midPoint);
    }
}
