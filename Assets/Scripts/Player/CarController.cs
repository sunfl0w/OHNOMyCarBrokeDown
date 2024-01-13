using UnityEngine;

public class AutoCarController : MonoBehaviour
{
    public float speed = 10f;
    public float decelerationRate = 2f;
    public Light carLight;
    private bool isDriving = true;

    private void Update()
    {
        if (isDriving)
        {
            // Drive the car forward
            transform.Translate(-Vector3.forward * speed * Time.deltaTime);

            // Check if the car should start slowing down
            if (Time.time >= 3f)
            {
                isDriving = false;
            }
        }
        else
        {
            // Gradually slow down and stop
            speed = Mathf.Max(0f, speed - decelerationRate * Time.deltaTime);
            transform.Translate(-Vector3.forward * speed * Time.deltaTime);

            // Dim the spotlight
            carLight.intensity = Mathf.Lerp(carLight.intensity, 0f, decelerationRate * Time.deltaTime);
        }
    }
}