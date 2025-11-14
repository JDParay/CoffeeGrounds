using UnityEngine;
using System;
using TMPro;

public class CGRPSEnlarger : MonoBehaviour
{
    public float hoverLift = 0.2f;
    public float moveSpeed = 5f;
    public string buttonName = "Rock";

    public Animator labelAnimator;            // word animation
    public Animator buttonAnimator;           // PNG animation (OPTIONAL)

    public event Action OnClicked;

    private Vector3 originalPosition;
    private bool isHovered = false;

    void Start()
    {
        originalPosition = transform.position;

        // Auto-find label animator
        if (labelAnimator == null)
            labelAnimator = GetComponentInChildren<Animator>();

        if (buttonAnimator == null)
        {
            Animator[] anims = GetComponentsInChildren<Animator>();
            if (anims.Length > 1)
                buttonAnimator = anims[1];
        }
    }

    void OnMouseEnter()
    {
        isHovered = true;
        Debug.Log($"Hovered: {buttonName}");

        if (labelAnimator != null)
            labelAnimator.SetTrigger("Show");

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Hover");
    }

    void OnMouseExit()
    {
        isHovered = false;

        if (labelAnimator != null)
            labelAnimator.Play("Idle");

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Idle");
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked: " + buttonName);
        OnClicked?.Invoke();
    }

    void Update()
    {
        Vector3 targetPos = isHovered
            ? originalPosition + Vector3.up * hoverLift
            : originalPosition;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
