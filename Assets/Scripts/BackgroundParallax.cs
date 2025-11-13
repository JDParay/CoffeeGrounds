using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{

    [Tooltip("The multiplier for movement. Closer layers have a higher value.")]
    public float parallaxMultiplier = 0.1f;

    private Camera mainCamera;

    private Vector3 startPosition; 

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;
    }

    void Update()
    {

        Vector3 mouseScreenPos = Input.mousePosition;

        Vector2 normalizedMouse = new Vector2(
            mouseScreenPos.x / Screen.width,
            mouseScreenPos.y / Screen.height
        );

        Vector2 centeredMouse = normalizedMouse - new Vector2(0.5f, 0.5f);

        Vector3 offset = new Vector3(
            centeredMouse.x * parallaxMultiplier,
            centeredMouse.y * parallaxMultiplier,
            0
        );

        transform.position = startPosition + offset;
    }
}