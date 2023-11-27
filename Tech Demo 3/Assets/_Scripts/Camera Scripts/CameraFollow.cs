using UnityEngine;

/// <summary>
/// Follows the specified object (In this case it is hard-wired to follow the player object
/// which it finds using the reference manager singleton.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // INFO: Time it takes to get from A to B
    [SerializeField] private float followDuration;

    private Transform followObject;

    private void Start()
    {
        followObject = ReferenceManager.Instance.playerObject.transform;
    }

    private void FixedUpdate()
    {
        Vector3 futurePos = new(followObject.position.x, followObject.position.y, transform.position.z);

        // INFO: Lerps between the current position and future position (Position of the object it's following)
        // to make the camera movement feel smoother and not snappy.
        transform.position = Vector3.Lerp(transform.position, futurePos, followDuration);
    }
}
