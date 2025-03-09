using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementpix : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float moveSpeed_tmp = 5f;
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
    public BoundaryChecker boundarychecker;
    public TextureBoundaryDetector textureboundarydetector;

    // 接触しているオブジェクトを管理するリスト
    private List<Collider2D> currentColliders = new List<Collider2D>();

    private bool allowSpace = true;

    public float threshold = 0.2f; // 判定に使う距離のしきい値

    void Start()
    {
        moveSpeed_tmp = moveSpeed;
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

        moveSpeed = moveSpeed_tmp;
        Vector2 nextPosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * moveSpeed * Time.fixedDeltaTime;


            // 渡すときだけ Vector3 に変換（z = 1）
            if (textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
            {
                moveDirection = new Vector2(moveX, moveY).normalized;
            }
            else
            {
                //角を曲がるときは速度を落とさないとうまくいかない
                //スピードを落として再判定
                moveSpeed = 0.5f;
                nextPosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * moveSpeed * Time.fixedDeltaTime;

                // 渡すときだけ Vector3 に変換（z = 1）
                if (textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
                {
                    moveDirection = new Vector2(moveX, moveY).normalized;
                }
                else
                {
                    moveDirection = Vector2.zero; // 移動不可なら停止
                }
            }
            //スペースキーが押されているとき
           if (Input.GetKey(KeyCode.Space)&&allowSpace)
           {
            moveSpeed = moveSpeed_tmp;
            moveDirection = new Vector2(moveX, moveY).normalized;

               //領域内から領域外にでる
               if (boundarychecker.isBoundary && !textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
               {
                Vector2 prePosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * (-1*moveSpeed) * Time.fixedDeltaTime;
                ExitArea(prePosition2D);
               }
               //領域外から領域内にもどる
               else if (!boundarychecker.isBoundary && textureboundarydetector.IsInTransparentArea(nextPosition2D))
               {
                   EnterArea(nextPosition2D);
                   boundarychecker.WarpToClosestBoundary();
                   moveDirection = Vector2.zero;
                   allowSpace = false;
               }
           //外にいるときに一
           if (!boundarychecker.isBoundary&& IsPointOnLine(nextPosition2D)&&(new Vector2(moveX,moveY) != Vector2.zero))
           {
                Debug.Log("同じ点を通りました");
                //最初の点に戻る
                Vector2 firstPoint = pathPoints[0];
                transform.position = firstPoint;
                pathPoints.Clear();
            }

                    // LineRendererで経路を描画
                    lineRenderer.positionCount = pathPoints.Count;
                    lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));

           }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!boundarychecker.isBoundary)
            {
                //最初の点に戻る
                Vector2 firstPoint = pathPoints[0];
                transform.position = firstPoint;
            }
            pathPoints.Clear();
            allowSpace = true;
        }


        if (moveDirection != Vector2.zero && moveDirection != lastDirection)
        {
            RecordPoint();
            lastDirection = moveDirection;
        }

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

    bool IsPointOnLineSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        float lengthAB = Vector2.Distance(a, b);
        float lengthAP = Vector2.Distance(a, p);
        float lengthPB = Vector2.Distance(p, b);

        // 線分上にあるか判定（誤差考慮）
        return Mathf.Approximately(lengthAB, lengthAP + lengthPB);
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

    void EnterArea(Vector2 next)
    {
        Debug.Log("領域に戻りました");
        StopPlayer();
        if (isOutsideMyArea)
        {
            pathPoints.Add(next); // 最終点
            GeneratePolygonCollider();
        }
        isOutsideMyArea = false;
        pathPoints.Clear();
    }

    void ExitArea(Vector2 pre)
    {
        pathPoints.Clear();
        Debug.Log("領域から出ました");
        isOutsideMyArea = true;
        pathPoints.Add(pre); // 最初の点を記録

    }


    bool IsPointOnLine(Vector2 point)
    {
        int count = lineRenderer.positionCount;

        // 最新のセグメントは除外するため、count - 1 までループ
        for (int i = 0; i < count - 100; i++)
        {
            Vector2 lineStart = lineRenderer.GetPosition(i);
            Vector2 lineEnd = lineRenderer.GetPosition(i + 1);

            if (IsPointNearLineSegment(lineStart, lineEnd, point))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPointNearLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        // 線分と点との距離を計算
        float distance = DistanceFromPointToLineSegment(lineStart, lineEnd, point);

        // 距離がしきい値以下なら重なっているとみなす
        return distance < threshold;
    }

    float DistanceFromPointToLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        float lineLength = (lineEnd - lineStart).magnitude;

        if (lineLength == 0)
            return Vector2.Distance(point, lineStart); // 始点と終点が同じなら始点までの距離

        float projection = Vector2.Dot(point - lineStart, lineEnd - lineStart) / lineLength;

        // プロジェクションが線分上に収まるかどうかを確認
        if (projection < 0)
            return Vector2.Distance(point, lineStart);
        if (projection > lineLength)
            return Vector2.Distance(point, lineEnd);

        // 線分上の最短距離
        Vector2 closestPoint = lineStart + projection * (lineEnd - lineStart).normalized;
        return Vector2.Distance(point, closestPoint);
    }




    void GeneratePolygonCollider()
    {
        //ここから
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
                //Vector2 pointB = CalculateRightAnglePoint(pathPoints[0], pathPoints[pathPoints.Count - 1]);
                //pathPoints.Add(pointB);
            }

        }
        //ここまでたぶんいらない

        GameObject newPoly = new GameObject("GeneratedPolygon");
        PolygonCollider2D polyCollider = newPoly.AddComponent<PolygonCollider2D>();

        polyCollider.points = pathPoints.ToArray();
        newPoly.tag = "myarea";

        // **デバッグ用ログ**
        Debug.Log($"生成された PolygonCollider2D の点の数: {polyCollider.points.Length}");
        foreach (Vector2 point in polyCollider.points)
        {
            Debug.Log($"点: {point}");
        }

        // **マスク処理を適用**
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