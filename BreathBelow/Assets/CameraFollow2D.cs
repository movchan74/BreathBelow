using UnityEngine;

public class CameraFollowX2D : MonoBehaviour
{
    public Transform target;                       // drag diver here
    public float smooth = 10f;
    public float xOffset = 0f;
    public float fixedY = 0f;
    public float fixedZ = -10f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = new Vector3(target.position.x + xOffset, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
    }
}
