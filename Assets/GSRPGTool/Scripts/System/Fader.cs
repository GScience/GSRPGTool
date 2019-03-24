using UnityEngine;

namespace RPGTool.System
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        private FaderState _faderState = FaderState.Idel;

        public CanvasGroup CanvasGroup { get; private set; }

        public bool IsFinished => _faderState == FaderState.Idel;

        public void FadeIn()
        {
            CanvasGroup.alpha = 1;
            _faderState = FaderState.FadingIn;
        }

        public void FadeOut()
        {
            CanvasGroup.alpha = 0;
            _faderState = FaderState.FadingOut;
        }

        private void Update()
        {
            if (_faderState == FaderState.FadingOut)
            {
                CanvasGroup.alpha += 2f * Time.deltaTime;

                if (!(CanvasGroup.alpha >= 1))
                    return;

                CanvasGroup.alpha = 1;
                _faderState = FaderState.Idel;
            }
            else if (_faderState == FaderState.FadingIn)
            {
                CanvasGroup.alpha -= 2f * Time.deltaTime;

                if (!(CanvasGroup.alpha <= 0))
                    return;

                CanvasGroup.alpha = 0;
                _faderState = FaderState.Idel;
            }
        }

        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        private enum FaderState
        {
            FadingIn,
            FadingOut,
            Idel
        }
    }
}