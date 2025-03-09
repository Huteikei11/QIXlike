using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public MaskController maskController;
    public GameObject maskObject; // `MaskController` ���A�^�b�`���ꂽ�I�u�W�F�N�g

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
            Debug.LogError("MaskController ��������܂���I�V�[�����ɔz�u����Ă��܂����H");
            return;
        }

        // �V�����|���S���R���C�_�[���쐬
        GameObject newMask = new GameObject("DynamicMask");
        PolygonCollider2D polyCollider = newMask.AddComponent<PolygonCollider2D>();

        // �K���Ȍ`���ݒ�i��: �l�p�`�j
        polyCollider.points = new Vector2[]
        {
            new Vector2(-0.2f, -0.2f),
            new Vector2(0.2f, -0.2f),
            new Vector2(0.2f, 0.2f),
            new Vector2(-0.2f, 0.2f)
        };
        Debug.Log("����");
        // ���������������s
        maskController.ApplyMask(polyCollider);
    }
}
