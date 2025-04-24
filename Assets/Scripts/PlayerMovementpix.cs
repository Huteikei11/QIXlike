using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementpix : MonoBehaviour
{
    public int life = 12;
    public float moveSpeed = 5f;
    private float moveSpeed_tmp = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private MaskController maskController;

    private bool isOutsideMyArea = false;
    private List<Vector2> pathPoints = new List<Vector2>();
    private Vector2 lastDirection;
    private Vector2 firstPoint;
    private bool isOut;

    public float MaxY;
    public float MinY;
    public float MaxX;
    public float MinX;

    public LineRenderer lineRenderer;  // LineRenderer
    public BoundaryChecker boundarychecker;
    public TextureBoundaryDetector textureboundarydetector;
    public LifeManager lifemanager;

    // 接触しているオブジェクトを管理するリスト
    private List<Collider2D> currentColliders = new List<Collider2D>();


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

    void FixedUpdate()
    {
        HandleMovementInput();
        MovePlayer();

        if (moveDirection != Vector2.zero && !boundarychecker.isBoundary)
        {
            // 領域外にいる間だけ記録
            if (pathPoints.Count == 0 || Vector2.Distance(pathPoints.Last(), transform.position) > 0.01f)
            {
                RecordPoint();
            }
        }
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            moveY = 0; // 水平方向の入力がある場合、垂直移動を無視
        }
        else if (moveY != 0)
        {
            moveX = 0; // 垂直方向の入力がある場合、水平方向を無視
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
                //moveDirection = moveDirection != Vector2.zero ? moveDirection : Vector2.zero;
                moveDirection = Vector2.zero; // 移動不可なら停止
            }
        }
        //スペースキーが押されているとき
        if (Input.GetKey(KeyCode.Space))
        {
            moveSpeed = moveSpeed_tmp;
            moveDirection = new Vector2(moveX, moveY).normalized;

            if (boundarychecker.isBoundary && textureboundarydetector.IsInTransparentArea(nextPosition2D)&&!isOut)
            {
                moveDirection = Vector2.zero;
            }

            //領域内から領域外にでる
            if (boundarychecker.isBoundary && !textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1)))&& !textureboundarydetector.IsInTransparentArea(nextPosition2D))
            {
                Vector2 prePosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * (-1*moveSpeed) * Time.fixedDeltaTime*5;
                //Vector3 prePosition2D = transform.position; // 逆方向に戻さない
                ExitArea(prePosition2D);
                isOut = true;
            }
            //領域外から領域内にもどる
            else if (!boundarychecker.isBoundary && textureboundarydetector.IsInTransparentArea(nextPosition2D))
            {
                EnterArea(nextPosition2D);
                boundarychecker.WarpToClosestBoundary();
                moveDirection = Vector2.zero;
                isOut = false;
            }
            //外にいるときに一
            if (!boundarychecker.isBoundary && IsPointOnLine(nextPosition2D) && (new Vector2(moveX, moveY) != Vector2.zero))
            {
                Debug.Log("同じ点を通りました");
                //最初の点に戻る
                if (pathPoints.Count > 0)
                {
                    firstPoint = pathPoints[0];
                }
                transform.position = firstPoint;
                StartCoroutine(ClearPathPointsNextFrame());
                isOut = false;
                boundarychecker.CheckMoveBoundary();//境界内に戻ったか確認
            }

            // LineRendererで経路を描画
            lineRenderer.positionCount = pathPoints.Count;
            lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));
        }
        else
        {
            if (!boundarychecker.isBoundary&&pathPoints.Count >= 2)
            {
                Debug.Log("スペースを離しました");
                //最初の点に戻る
                Vector2 firstPoint = pathPoints[0];
                transform.position = firstPoint;
                pathPoints.Clear();

                // LineRendererで経路を描画
                lineRenderer.positionCount = pathPoints.Count;
                lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));
                isOut = false;
                boundarychecker.CheckMoveBoundary();//境界内に戻ったか確認
            }
            
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
        Debug.Log($"{transform.position}");
        pathPoints.Add(transform.position);
    }

    void CloseShape()
    {
        if (pathPoints.Count > 2 && pathPoints[0] != pathPoints[pathPoints.Count - 1])
        {
            pathPoints.Add(pathPoints[0]); // 最初の点を最後に追加して閉じる
        }
    }

    void DebugPathPoints()
    {
        foreach (Vector2 point in pathPoints)
        {
            Debug.Log($"Point: {point}");
        }
    }

    private Vector3[] ConvertToVector3Array(List<Vector2> pathPoints)
    {
        //pathPoints.Add(transform.position);
        Vector3[] positions = new Vector3[pathPoints.Count];
        for (int i = 0; i < pathPoints.Count; i++)
        {
            positions[i] = new Vector3(pathPoints[i].x, pathPoints[i].y, 0);  // z軸は0に設定
        }
        return positions;
    }

    void EnterArea(Vector2 next)
    {
        Debug.Log($"[記録点数]: {pathPoints.Count}");
        Debug.Log("領域に戻りました");
        StopPlayer();
        if (isOutsideMyArea)
        {
            pathPoints.Add(next);
            //RecordPoint();//最終点
            FixStraightLineToRectangle();//一直線対策
            CloseShape();//ちゃんと閉じる
            DebugPathPoints();//デバッグ
            GeneratePolygonCollider();
        }
        isOutsideMyArea = false;
        Debug.Log($"[記録点数]: {pathPoints.Count}");
        // Coroutine で次のフレームにクリア
        //StartCoroutine(ClearPathPointsNextFrame());


    }

    IEnumerator ClearPathPointsNextFrame()
    {
        yield return null; // 1フレーム待機
        pathPoints.Clear();
    }

    void ExitArea(Vector3 pre)
    {
        //pathPoints.Clear();
        RecordPoint(); // 出た直後の点
        pathPoints.Add(pre); // 直前の点（領域内の出口点）
        Debug.Log("領域から出ました");
        isOutsideMyArea = true;
        //pathPoints.Add(pre); // 最初の点を記録

    }


    bool IsPointOnLine(Vector2 point)
    {
        int count = lineRenderer.positionCount;

        // 最新のセグメントは除外するため、count - 1 までループ
        for (int i = 0; i < count - 10; i++)
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

    public bool IsEnemyOnLine(Vector2 point)
    {
        int count = lineRenderer.positionCount;

        // 最新のセグメントは除外しない
        for (int i = 0; i < count-1; i++)
        {
            Vector2 lineStart = lineRenderer.GetPosition(i);
            Vector2 lineEnd = lineRenderer.GetPosition(i + 1);

            if (IsPointNearLineSegment(lineStart, lineEnd, point))
            {
                DeathPlayer();
                return true;
            }
        }
        return false;
    }

    public void DeathPlayer()
    {
        life -= 1;
        lifemanager.AddLife(-1);
        //最初の点に戻る
        Vector2 firstPoint = pathPoints[0];
        transform.position = firstPoint;
        pathPoints.Clear();
        //if (!boundarychecker.isBoundary)
        boundarychecker.CheckMoveBoundary();//境界内に戻ったか確認
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
        

        GameObject newPoly = new GameObject("GeneratedPolygon");
        PolygonCollider2D polyCollider = newPoly.AddComponent<PolygonCollider2D>();

        polyCollider.points = pathPoints.ToArray();
        newPoly.tag = "myarea";

        // **デバッグ用ログ**
        Debug.Log($"生成された PolygonCollider2D の点の数: {polyCollider.points.Length}");
        foreach (Vector2 point in polyCollider.points)
        {
            //Debug.Log($"点: {point}");
        }

        // **マスク処理を適用**
        if (maskController != null)
        {

            maskController.ApplyMask(polyCollider);
        }

        Destroy(newPoly);
        pathPoints.Clear();
        StartCoroutine(CheckAllEnemies());
    }

    void FixStraightLineToRectangle()
    {
        if (pathPoints.Count < 2) return;

        bool allXSame = true;
        bool allYSame = true;

        float firstX = pathPoints[0].x;
        float firstY = pathPoints[0].y;

        foreach (Vector2 point in pathPoints)
        {
            if (point.x != firstX) allXSame = false;
            if (point.y != firstY) allYSame = false;
        }

        // すでに多角形になっているなら処理しない
        if (!allXSame && !allYSame) return;

        Vector2 firstPoint = pathPoints[0];
        Vector2 lastPoint = pathPoints[pathPoints.Count - 1];

        float offset = 0.02f; // 少しだけずらす距離（必要に応じて調整）

        if (allXSame)
        {
            // xがすべて同じ：縦線 → 横方向に補助点を追加
            pathPoints.Add(new Vector2(lastPoint.x + offset, lastPoint.y));
            pathPoints.Add(new Vector2(firstPoint.x + offset, firstPoint.y));
        }
        else if (allYSame)
        {
            // yがすべて同じ：横線 → 縦方向に補助点を追加
            pathPoints.Add(new Vector2(lastPoint.x, lastPoint.y + offset));
            pathPoints.Add(new Vector2(firstPoint.x, firstPoint.y + offset));
        }
    }

    IEnumerator CheckAllEnemies()
    {
        yield return null; // 1フレーム待機
        // Enemyタグを持つ全てのオブジェクトを取得
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 各敵オブジェクトのCheckAreaを呼び出し
        foreach (GameObject enemy in enemies)
        {
            EnemyMover enemyMover = enemy.GetComponent<EnemyMover>();
            if (enemyMover != null)
            {
                enemyMover.CheckArea();
            }
        }
    }


}