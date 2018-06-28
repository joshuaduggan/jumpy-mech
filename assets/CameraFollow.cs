using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform heroTrans;
    public Vector3 offset;
    public float smoothSpeedX;
    public float smoothSpeedY;


    void FixedUpdate () {
        transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, heroTrans.position.x + offset.x, smoothSpeedX),
                Mathf.Lerp(transform.position.y, heroTrans.position.y + offset.y, smoothSpeedY),
                offset.z);
    }
}
