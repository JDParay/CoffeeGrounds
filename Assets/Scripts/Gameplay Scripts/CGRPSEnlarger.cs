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

    // NEW ------------------------------
    public RPSGameController.RPSChoice choiceType;
    public BoxCollider2D boxCollider;
    public GameObject blockerPNG;
    // ----------------------------------

    private Vector3 originalPosition;
    private bool isHovered = false;

    private bool skillUnlocked = false; 
    private bool skillUsed = false;

    public bool interactable = true;

    void Start()
    {
        originalPosition = transform.position;

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
    }

    // ---------------------------------------------------------
    //                      HOVER
    // ---------------------------------------------------------

    void OnMouseEnter()
    {
        if (!boxCollider.enabled) return;
        if (!interactable) return;

        isHovered = true;

        // Hover anim
        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Hover", true);
            buttonAnimator.SetBool("Notif", false); // stop notif while hovering
        }

        if (labelAnimator != null)
            labelAnimator.SetTrigger("Show");
    }

    void OnMouseExit()
    {
        if (!boxCollider.enabled) return;

        isHovered = false;

        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Hover", false);

            // If skill not used, notif resumes
            if (skillUnlocked && !skillUsed)
                buttonAnimator.SetBool("Notif", true);
            else
                buttonAnimator.SetBool("Notif", false);
        }

        if (labelAnimator != null)
            labelAnimator.Play("Idle");
    }

    // ---------------------------------------------------------
    //                      CLICK
    // ---------------------------------------------------------

    void OnMouseDown()
    {
        if (!boxCollider.enabled) return;
        if (!interactable) return;

        Debug.Log("Clicked: " + buttonName);

        OnClicked?.Invoke();

        skillUsed = true;

        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Notif", false);
            buttonAnimator.SetTrigger("Used");    // notif → idle
        }
    }

    // ---------------------------------------------------------
    //                      MOVEMENT
    // ---------------------------------------------------------

    void Update()
    {
        Vector3 targetPos = isHovered
            ? originalPosition + Vector3.up * hoverLift
            : originalPosition;

        transform.position = Vector3.Lerp(transform.position, targetPos,
            Time.deltaTime * moveSpeed);
    }

    // ---------------------------------------------------------
    //          CALLED BY RPSGameController WHEN UNLOCKED
    // ---------------------------------------------------------

    public void NotifySkillUnlocked()
    {
        skillUnlocked = true;

        if (buttonAnimator != null)
            buttonAnimator.SetBool("Notif", true);  // idle → notif
    }

    public void ResetSkillState()
    {
        skillUnlocked = false;
        skillUsed = false;

        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Notif", false);
            buttonAnimator.SetBool("Hover", false);
        }
    }

    public void SetInteractable(bool state)
    {
        interactable = state;

        if (blockerPNG != null)
            blockerPNG.SetActive(!state);
    }
}
