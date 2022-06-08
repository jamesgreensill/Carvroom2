using Carvroom.Data;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 MinMaxSpeed;
    public float SpeedDamper;

    public Rigidbody TargetRigidbody;

    public Transform TargetPosition;
    public Transform TargetLookAt;

    public LayerMask CamOcclusionMask;

    private Vector3 m_CamPosition;

    private void Start()
    {
        unsafe
        {
            fixed (float* ptr = &SpeedDamper)
            {
                DebugSystem.Instance.Add(ptr, "Camera Speed Damping", DebugForms.Slider, 0, 100);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetOffset = TargetPosition.position;

        m_CamPosition = targetOffset;

        if (Physics.Linecast(TargetLookAt.position, m_CamPosition, out var wallHit, CamOcclusionMask))
        {
            m_CamPosition = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, m_CamPosition.y, wallHit.point.z + wallHit.normal.z * 0.5f);
        }

        var speed = Mathf.Clamp(TargetRigidbody.velocity.magnitude / SpeedDamper, MinMaxSpeed.x, MinMaxSpeed.y);
        transform.position = Vector3.Lerp(transform.position, m_CamPosition, Time.deltaTime * speed);
        transform.LookAt(TargetLookAt);
    }
}