using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class PasswordVisibilityToggle : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField passwordField;
        [SerializeField] 
        private GameObject showIcon;
        [SerializeField] 
        private GameObject hideIcon;
        
        private bool _isPasswordVisible = false;

        public void TogglePassword()
        {
            if (_isPasswordVisible)
            {
                passwordField.contentType = TMP_InputField.ContentType.Password;

                hideIcon.SetActive(true);
                showIcon.SetActive(false);
            }
            else
            {
                passwordField.contentType = TMP_InputField.ContentType.Standard;

                hideIcon.SetActive(false);
                showIcon.SetActive(true);
            }

            _isPasswordVisible = !_isPasswordVisible;

            passwordField.ForceLabelUpdate();
        }
    }
}