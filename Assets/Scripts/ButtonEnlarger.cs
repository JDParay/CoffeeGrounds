using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEnlarger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float sizeDifference = 1.2f;
    public float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private RectTransform rectTransform;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(originalScale * sizeDifference);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(originalScale);
    }

    void StartScale(Vector3 newTarget)
    {
        targetScale = newTarget;
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleAnimation());
    }

    IEnumerator ScaleAnimation()
    {
        while (Vector3.Distance(rectTransform.localScale, targetScale) > 0.001f)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
            yield return null;
        }

        rectTransform.localScale = targetScale;
    }
}
