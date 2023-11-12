using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody2D rb2D;

    private Vector2 movementInput;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInputAxis();
    }

    private void FixedUpdate()
    {
        movementInput.Normalize();
        rb2D.velocity = new Vector2(movementInput.x * movementSpeed, movementInput.y * movementSpeed);
    }

    private void GetInputAxis()
    {
#if UNITY_EDITOR
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
#endif

        if (ReferenceManager.Instance.joystickController.GetJoystickInput() != Vector2.zero)
        {
            movementInput = ReferenceManager.Instance.joystickController.GetJoystickInput();
        }
    }

    public Vector2 GetMovementInput()
    {
        return movementInput;
    }
}
