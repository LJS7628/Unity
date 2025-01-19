using UnityEngine;

public class Warp_Cursor : MonoBehaviour
{
    private float traceDistance;
    public float TraceDistance { set => traceDistance = value; }

    private LayerMask mask;
    public LayerMask Mask { set => mask = value; }

    private void Awake()
    {
        Transform t = GameObject.Find("Envirments").transform;
        transform.SetParent(t, false);
    }

    private void Update()
    {
        Vector3 position;
        Vector3 normal;

        //지형물이 아니면 리턴
        if (CameraHelpers.GetCursorLocation(out position, out normal, traceDistance, mask) == false)
            return;

        position += normal * 0.05f;
        transform.localPosition = position;

        Vector3 up = Quaternion.Euler(-90, 0, 0) * Vector3.up;
        Quaternion q = Quaternion.FromToRotation(up, normal);
        transform.localRotation = q;
    }
}
