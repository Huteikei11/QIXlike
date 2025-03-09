using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public MaskController maskController;
    public GameObject maskObject; // `MaskController` がアタッチされたオブジェクト

    void Start()
    {
        TestCreate();
    }

    public void TestCreate() 
    {
        if (maskController == null)
        {
            maskController = FindObjectOfType<MaskController>();
        }

        if (maskController == null)
        {
            Debug.LogError("MaskController が見つかりません！シーン内に配置されていますか？");
            return;
        }

        // 新しいポリゴンコライダーを作成
        GameObject newMask = new GameObject("DynamicMask");
        PolygonCollider2D polyCollider = newMask.AddComponent<PolygonCollider2D>();

        // 適当な形状を設定（例: 四角形）
        polyCollider.points = new Vector2[]
        {
            new Vector2(-0.2f, -0.2f),
            new Vector2(0.2f, -0.2f),
            new Vector2(0.2f, 0.2f),
            new Vector2(-0.2f, 0.2f)
        };
        Debug.Log("生成");
        // 透明化処理を実行
        maskController.ApplyMask(polyCollider);
    }
}
