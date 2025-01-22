using DG.Tweening;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(LayoutElement))]
    public class AccordionElement : MonoBehaviour
    {
        public enum Transition
        {
            Instant,
            Tween
        }

        public enum State
        {
            Collapsed,
            Expanded
        }

        [SerializeField] private float _minHeight = 18f;
        [SerializeField] private Transition _transition = Transition.Tween;
        [SerializeField] private float _transitionDuration = 0.3f;
        [SerializeField] private State _currentState = State.Expanded;

        public Transition transition
        {
            get { return _transition; }
            set { _transition = value; }
        }

        public float transitionDuration
        {
            get { return _transitionDuration; }
            set { _transitionDuration = value; }
        }

        private RectTransform _rectTransform;
        private Toggle _toggle;
        private LayoutElement _layoutElement;

        protected virtual void Awake()
        {
            _rectTransform = transform as RectTransform;
            _layoutElement = gameObject.GetComponent<LayoutElement>();
            _toggle = gameObject.GetComponent<Toggle>();

            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(OnValueChanged);
            }
        }

        protected virtual void OnValidate()
        {
            LayoutElement le = gameObject.GetComponent<LayoutElement>();

            if (le != null)
            {
                le.preferredHeight = (_currentState == State.Expanded) ? -1f : _minHeight;
            }
        }

        public void OnValueChanged(bool state)
        {
            if (!enabled || !gameObject.activeInHierarchy)
                return;

            TransitionToState(state ? State.Expanded : State.Collapsed);
        }

        public void TransitionToState(State state)
        {
            if (_layoutElement == null)
                return;

            _currentState = state;

            var header = transform.GetChild(0);
            var arrow = header.GetChild(0);

            if (_transition == Transition.Instant)
            {
                _layoutElement.preferredHeight = (state == State.Expanded) ? -1f : _minHeight;
            }
            else if (_transition == Transition.Tween)
            {
                if (state == State.Expanded)
                {
                    StartTween(_layoutElement.preferredHeight, GetExpandedHeight());
                    GetComponent<Image>().color = new Color32(29, 27, 42, 255);

                    arrow.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 90);
                }
                else
                {
                    StartTween((_layoutElement.preferredHeight == -1f) ? _rectTransform.rect.height : _layoutElement.preferredHeight, _minHeight);
                    GetComponent<Image>().color = new Color32(29, 27, 42, 0);

                    arrow.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -90);
                }
            }
        }

        protected float GetExpandedHeight()
        {
            if (_layoutElement == null)
                return _minHeight;

            float originalPrefH = _layoutElement.preferredHeight;
            _layoutElement.preferredHeight = -1f;
            float h = LayoutUtility.GetPreferredHeight(_rectTransform);
            _layoutElement.preferredHeight = originalPrefH;

            return h;
        }

        protected void StartTween(float startFloat, float targetFloat)
        {
            DOTween.Kill(this);

            DOTween.To(() => startFloat, value => SetHeight(value), targetFloat, _transitionDuration)
                   .SetEase(Ease.Linear)
                   .SetTarget(this);
        }

        protected void SetHeight(float height)
        {
            if (_layoutElement == null)
                return;

            _layoutElement.preferredHeight = height;
        }
    }
}
