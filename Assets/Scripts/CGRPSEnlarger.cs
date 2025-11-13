using UnityEngine;
using System;
using TMPro;

public class CGRPSEnlarger : MonoBehaviour
{
    public float hoverLift = 0.2f;      // how high it moves on hover
    public float moveSpeed = 5f;        // how smooth it moves
    public string buttonName = "Rock";  // custom name (e.g., Rock, Paper, Scissors)
    public Animator labelAnimator; 
    public event Action OnClicked;
    private Vector3 originalPosition;
    private bool isHovered = false;

    void Start()
    {
        originalPosition = transform.position;
        labelAnimator = GetComponentInChildren<Animator>();
    }

    void OnMouseEnter()
    {
        isHovered = true;
        Debug.Log($"Hovered: {buttonName}");

        labelAnimator.Play("word_show");
    }

    void OnMouseExit()
    {
        isHovered = false;
        labelAnimator.Play("Idle");
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
