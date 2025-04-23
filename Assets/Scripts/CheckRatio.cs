using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween���g�p���邽�߂̖��O���

public class CheckRation : MonoBehaviour
{
    public float ratio;
    public TextMeshProUGUI areaRatio;
    public TextMeshProUGUI stageClearText; // STAGE CLEAR�p��TextMeshPro
    public List<SpriteRenderer> spriteRenderersToFade; // �t�F�[�h�A�E�g������SpriteRenderer�̃��X�g
    public CanvasGroup transitionCanvasGroup; // �g�����W�V�����p��CanvasGroup
    public float clearRatio = 0.6f; // �N���A�����̔䗦

    public float CalculateTransparencyRatio(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture2D��null�ł��B");
            return 0f;
        }

        Color[] pixels = texture.GetPixels();
        int totalPixels = pixels.Length;
        int transparentPixels = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a == 0f) // �A���t�@�l��0�̏ꍇ�A�����Ƃ݂Ȃ�
            {
                transparentPixels++;
            }
        }
        ratio = (float)transparentPixels / totalPixels;
        DisplayRatioAsPercent();

        CheckClear(); // �N���A����
        return ratio;
    }

    public void DisplayRatioAsPercent()
    {
        if (areaRatio == null)
        {
            Debug.LogWarning("TextMeshProUGUI���ݒ肳��Ă��܂���B");
            return;
        }

        // ratio���p�[�Z���g�\�L�ɕϊ�
        float percent = ratio * 100f;
        float clearPercent = clearRatio * 100f; // �N���A�����̃p�[�Z���g

        // TextMeshProUGUI�ɕ\��
        areaRatio.text = $"{percent:F1}/{clearPercent:F0}"; // �����_�ȉ�2���܂ŕ\��
    }

    private void CheckClear()
    {
        if (ratio > clearRatio)
        {
            Debug.Log("�N���A�����𖞂����܂����I");
            StartCoroutine(StageClearSequence());
        }
    }

    private IEnumerator StageClearSequence()
    {


        // STAGE CLEAR�̕�����\��
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.DOFade(1f, 0.5f); // �t�F�[�h�C��
        }
        else
        {
            Debug.LogWarning("stageClearText���ݒ肳��Ă��܂���B");
        }

        // 2�b�ԑҋ@
        yield return new WaitForSeconds(1f);
        // Player��Enemy�̃I�u�W�F�N�g���\��
        HidePlayerAndEnemyObjects();

        // STAGE CLEAR�̕��������X�ɓ����ɂ���
        if (stageClearText != null)
        {
            stageClearText.gameObject.SetActive(true);
            stageClearText.DOFade(0f, 0.5f); // �t�F�[�h�C��
        }
        // SpriteRenderer�����X�ɓ����ɂ���
        foreach (var spriteRenderer in spriteRenderersToFade)
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FadeOutSprite(spriteRenderer));
            }
            else
            {
                Debug.LogWarning("spriteRenderersToFade�̒���null���܂܂�Ă��܂��B");
            }
        }

        // �ҋ@
        yield return new WaitForSeconds(1f);

        // �L�[���͑҂�
        yield return new WaitUntil(() => Input.anyKeyDown);

        // �g�����W�V�����J�n
        yield return StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator FadeOutSprite(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null) yield break;

        float duration = 1f; // �t�F�[�h�A�E�g�̎���
        float elapsed = 0f;

        Color originalColor = spriteRenderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        if (transitionCanvasGroup != null)
        {
            // ��ʂ����X�ɈÂ�����
             yield return transitionCanvasGroup.DOFade(1f, 1f).WaitForCompletion();

            // �V�[���J��

            ChangeScene();

            // �V�[���J�ڌ�A��ʂ����X�ɖ��邭����
            yield return transitionCanvasGroup.DOFade(0f, 1f).WaitForCompletion();
         }
        else
        {
        Debug.LogWarning("transitionCanvasGroup���ݒ肳��Ă��܂���B");
        }
    }
    private void ChangeScene()
    {
        try
        {
            // SaveManager����level��life���擾���ď�����
            int level = SaveManager.Instance.GetLevel();
            int life = SaveManager.Instance.GetLifeCount();

            SaveManager.Instance.SetLevel(level+1);
            SaveManager.Instance.SetLifeCount(life);

            Debug.Log($"�Q�[�����J�n���܂� ���x��: {level}, ���C�t: {life}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveManager����f�[�^���擾�ł��܂���ł���: {ex.Message}");
            return; // ��O�����������ꍇ�A�Q�[���J�n�������X�L�b�v
        }

        // �V�[�������[�h
        SceneManager.LoadScene("Main");
    }

    private void HidePlayerAndEnemyObjects()
    {
        // Player�^�O�̃I�u�W�F�N�g���\��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.SetActive(false);
        }

        // Enemy�^�O�̃I�u�W�F�N�g���\��
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }

        Debug.Log("Player��Enemy�̃I�u�W�F�N�g���\���ɂ��܂����B");
    }
}
