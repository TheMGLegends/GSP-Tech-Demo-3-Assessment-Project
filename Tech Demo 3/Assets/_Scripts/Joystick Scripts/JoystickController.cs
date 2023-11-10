using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            joystickHandle.rectTransform.anchoredPosition = Vector2.Lerp(joystickHandle.rectTransform.anchoredPosition, Vector2.zero, returnSmoothing);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        returnJoystick = false;
        Vector2 backgroundSize = joystickBackground.rectTransform.sizeDelta;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.rectTransform, eventData.position, Camera.main, out joystickInput))
        {
            joystickInput.x /= backgroundSize.x;
            joystickInput.y /= backgroundSize.y;

            if (joystickInput.magnitude > 1)
            {
                joystickInput = joystickInput.normalized;
            }

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
