using UnityEngine;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{
    public float fadeDuration = 1f; // �t�F�[�h�C���E�A�E�g�̎���
    public float moveDistance = 1f; // �㉺�ړ��̋���
    public float moveDuration = 2f; // �㉺�ړ��̎���
    public bool isLogo; // ���S���ǂ����𔻒�

    private SpriteRenderer spriteRenderer; // �t�F�[�h�p
    private Transform spriteTransform; // �ړ��p

    private void Awake()
    {
        // �K�v�ȃR���|�[�l���g���擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteTransform = GetComponent<Transform>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer���A�^�b�`����Ă��܂���B");
        }

        if (spriteTransform == null)
        {
            Debug.LogError("Transform���A�^�b�`����Ă��܂���B");
        }
    }

    private void Start()
    {
        if (isLogo)
        {
            // ���S�̃A�j���[�V�������J�n
            StartMoveAnimation();
        }
        else
        {
            // �����̃A�j���[�V�������J�n
            StartFadeAnimation();
        }
    }

    private void StartFadeAnimation()
    {
        if (spriteRenderer != null)
        {
            // �t�F�[�h�C���E�A�E�g�����[�v
            spriteRenderer.DOFade(0f, fadeDuration)
                .SetLoops(-1, LoopType.Yoyo) // �������[�v��Yoyo�i�s�����藈����j
                .SetEase(Ease.InOutSine); // ���炩�ȃC�[�W���O
        }
    }

    private void StartMoveAnimation()
    {
        if (spriteTransform != null)
        {
            // �㉺�ړ������[�v
            spriteTransform.DOMoveY(spriteTransform.position.y + moveDistance, moveDuration)
                .SetLoops(-1, LoopType.Yoyo) // �������[�v��Yoyo�i�s�����藈����j
                .SetEase(Ease.InOutSine); // ���炩�ȃC�[�W���O
        }
    }
}
