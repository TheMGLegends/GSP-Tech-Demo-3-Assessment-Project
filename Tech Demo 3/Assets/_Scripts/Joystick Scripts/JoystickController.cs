using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Controls what happens when the joystick is being dragged, pressed and released by utilizing
/// interfaces that force us to implement the methods that are in the interfaces
/// </summary>
public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Range(1f, 5f)]
    [SerializeField] private float backgroundSizeOffset;
    [SerializeField] private float returnSmoothing;

    private Image joystickBackground;
    private Image joystickHandle;
    private Vector2 joystickInput;
    private bool returnJoystick;

    private void Start()
    {
        joystickBackground = GetComponent<Image>();
        joystickHandle = transform.GetChild(0).GetComponent<Image>();
        returnJoystick = false;
    }

    private void Update()
    {
        if (returnJoystick)
        {
            // INFO: Return Joystick true implies user has taken their finger off meaning we do not want to detect any input
            joystickInput = Vector2.zero;
            // INFO: Lerps the joystick back to its centre position instead of snapping it back
            joystickHandle.rectTransform.anchoredPosition = Vector2.Lerp(joystickHandle.rectTransform.anchoredPosition, Vector2.zero, returnSmoothing);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        returnJoystick = false;
        Vector2 backgroundSize = joystickBackground.rectTransform.sizeDelta;

        // INFO: Converts screen space to local space coordinates of the specified rect (Joystick Background) and then detects the position of where the
        // user has touched, this then gets outputted into joystick input so that it can ultimately be used to move the player
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.rectTransform, eventData.position, Camera.main, out joystickInput))
        {
            // INFO: Scaling input based on the overall size of the background
            joystickInput.x /= backgroundSize.x;
            joystickInput.y /= backgroundSize.y;

            // INFO: Normalizing input when magnitude greater than 1 to prevent faster speeds
            if (joystickInput.magnitude > 1)
                joystickInput = joystickInput.normalized;

            joystickHandle.rectTransform.anchoredPosition = new Vector2(joystickInput.x * (backgroundSize.x / backgroundSizeOffset), 
                                                                        joystickInput.y * (backgroundSize.y / backgroundSizeOffset));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickInput = Vector2.zero;
        returnJoystick = true;     
    }

    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }
}
