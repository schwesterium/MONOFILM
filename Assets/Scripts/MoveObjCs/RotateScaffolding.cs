using UnityEngine;

public class RotateScaffolding : MonoBehaviour
{
    [SerializeField]
    [Tooltip("回転速度と軸を指定する")]
    private Vector3 rotateSpeed = Vector3.zero;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //オブジェクトをrotateSpeedで回転させる
    void FixedUpdate()
    {
        Quaternion quaternion = Quaternion.Euler(rotateSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * quaternion);
    }
}
