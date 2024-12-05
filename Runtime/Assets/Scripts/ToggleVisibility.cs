using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameObject;

    public void SetActive()
    {
        _gameObject.SetActive(true);
    }

    public void SetInactive()
    {
        _gameObject.SetActive(false);
    }
}
