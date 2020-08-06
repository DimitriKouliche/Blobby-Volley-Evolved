using UnityEngine;
public class CameraShake : MonoBehaviour
{
    public float Strength;
    public float Duration;
    private Vector3 _initialCameraPosition;
    private float _remainingShakeTime;

    public void Shake()
    {
        _remainingShakeTime = Duration;
        enabled = true;
    }
    private void Awake()
    {
        _initialCameraPosition = transform.localPosition;
        enabled = false;
    }
    private void Update()
    {
        if (_remainingShakeTime <= 0)
        {
            transform.localPosition = _initialCameraPosition;
            enabled = false;
        }

        transform.Translate(Random.insideUnitCircle * Strength);

        _remainingShakeTime -= Time.deltaTime;
    }
}