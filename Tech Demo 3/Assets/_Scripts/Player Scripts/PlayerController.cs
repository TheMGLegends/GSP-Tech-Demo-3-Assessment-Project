using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody2D rb2D;

    private Vector2 movementInput;
    public Vector2 GetMovementInput() => movementInput;

    private bool isTargeting;
    public bool GetIsTargeting() => isTargeting;

    private GameObject target;
    public GameObject GetTarget() => target;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInputAxis();
        TargetAction();
    }

    private void FixedUpdate()
    {
        movementInput.Normalize();
        rb2D.velocity = new Vector2(movementInput.x * movementSpeed, movementInput.y * movementSpeed);
    }

    private void TargetAction()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) return;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            LockOn(touchPosition);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LockOn(touchPosition);
        }
#endif
    }

    private void LockOn(Vector3 touchPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit != false && hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
                isTargeting = true;

                Debug.Log(target.name);
                Debug.Log(isTargeting);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                target = null;
                isTargeting = false;

                Debug.Log(isTargeting);
            }
        }
    }

    private void GetInputAxis()
    {
#if UNITY_EDITOR
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
#endif

        if (ReferenceManager.Instance.joystickController.GetJoystickInput() != Vector2.zero)
            movementInput = ReferenceManager.Instance.joystickController.GetJoystickInput();
    }
}
