using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    private bool isTargeting;
    private GameObject target;

    private void Update()
    {
        LockOnTarget();
    }

    private void LockOnTarget()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                return;
            }

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            LockOnCalculation(touchPosition);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LockOnCalculation(touchPosition);
        }
#endif
    }

    private void LockOnCalculation(Vector3 touchPosition)
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

    public bool IsTargeting()
    {
        return isTargeting;
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
