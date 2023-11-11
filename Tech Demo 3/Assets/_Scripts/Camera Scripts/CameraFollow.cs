using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed;

    private Transform followPlayer;

    private void Start()
    {
        followPlayer = ReferenceManager.Instance.playerObject.transform;
    }

    private void FixedUpdate()
    {
        Vector3 futurePos = new(followPlayer.position.x, followPlayer.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, futurePos, followSpeed);
    }
}
