using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1) * -150 * Time.deltaTime);
    }
}
