using UnityEngine;
using System;
using TMPro;

public class CGRPSEnlarger : MonoBehaviour
{
    public float hoverLift = 0.2f;
    public float moveSpeed = 5f;
    public string buttonName = "Rock";

    public Animator labelAnimator;   
    public Animator buttonAnimator;  

    public event Action OnClicked;
    public UnityEngine.UI.Button uiButton;

    // NEW ------------------------------
    public RPSGameController.RPSChoice choiceType;
    public BoxCollider2D boxCollider;
    public GameObject blockerPNG;
    // ----------------------------------

    private Vector3 originalPosition;
    private bool isHovered = false;
    public bool interactable = true;

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

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (blockerPNG != null)
            blockerPNG.SetActive(false);

        if (uiButton != null)
            uiButton.onClick.AddListener(() => OnClicked?.Invoke());
    }

    void OnMouseEnter()
    {
        if (!boxCollider.enabled) return; // prevents hover on disabled

        isHovered = true;

        if (labelAnimator != null)
            labelAnimator.SetTrigger("Show");

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Hover");
    }

    void OnMouseExit()
    {
        if (!boxCollider.enabled) return;

        isHovered = false;

        if (labelAnimator != null)
            labelAnimator.Play("Idle");

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Idle");
    }

    void OnMouseDown()
    {
        if (!boxCollider.enabled) return;
        if (!interactable) return;

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

    public void SetInteractable(bool state)
    {
        interactable = state;

        if (blockerPNG != null)
            blockerPNG.SetActive(!state); // show overlay if not interactable
    }
}
