using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public static class UIUtils
    {
        public static void SetupButton(Button button, UnityAction callback, Soundbank soundbank)
        {
            SetupButton(button, callback, null, null, true, soundbank);
        }

        public static void SetupButton(
            Button button,
            UnityAction callback,
            UnityAction onFocusInCallback,
            UnityAction onFocusOutCallback,
            bool hasPopupAnimation,
            Soundbank soundbank)
        {
            button.RegisterCallback<ClickEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                soundbank.PlaySound(soundbank.uiHover);
                callback.Invoke();
            });
            button.RegisterCallback<NavigationSubmitEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                soundbank.PlaySound(soundbank.uiDecision);
                callback.Invoke();
            });

            button.RegisterCallback<FocusInEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                soundbank.PlaySound(soundbank.uiHover);
                onFocusInCallback?.Invoke();
            });
            button.RegisterCallback<PointerEnterEvent>(ev =>
            {

                soundbank.PlaySound(soundbank.uiHover);
                onFocusInCallback?.Invoke();
            });


            button.RegisterCallback<FocusOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
            button.RegisterCallback<PointerOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
        }

        public static void PlayPopAnimation(VisualElement button)
        {
            PlayPopAnimation(button, new(0.85f, 0.85f, 0.85f), 0.5f);
        }

        public static void PlayPopAnimation(VisualElement button, Vector3 startingScale, float duration)
        {
            button.transform.scale = Vector3.one;

            DOTween.To(
                () => startingScale,
                scale => button.transform.scale = scale,
                Vector3.one,
                duration
            ).SetEase(Ease.OutElastic);
        }

        public static void ScrollToLastPosition(int currentIndex, ScrollView scrollView, UnityAction onFinish)
        {
            VisualElement lastElement = null;

            int lastScrollElementIndex = currentIndex;

            if (lastScrollElementIndex != -1 && scrollView?.childCount > 0)
            {
                while (lastScrollElementIndex >= 0 && lastScrollElementIndex + 1 < scrollView.childCount && lastElement == null)
                {
                    lastElement = scrollView?.ElementAt(lastScrollElementIndex + 1);

                    if (lastElement != null)
                    {
                        lastElement.Focus();
                        scrollView.ScrollTo(lastElement);
                        break;
                    }
                    else
                    {
                        lastScrollElementIndex--;
                    }
                }

            }

            onFinish();
        }


        public static void DisplayInsufficientBarBackgroundColor(Color originalColor, VisualElement target)
        {
            Color blinkColor = Color.red; // Change to Color.grey if needed

            // Sequence for the blink effect
            Sequence blinkSequence = DOTween.Sequence();
            blinkSequence.Append(
                DOTween.To(() => (Color)target.style.backgroundColor.value,
                           x => target.style.backgroundColor = new StyleColor(x),
                           blinkColor, 0.5f)
                       .SetEase(Ease.InOutFlash))
                       .OnComplete(() =>
                       {
                           target.style.backgroundColor = originalColor;
                       });
        }

        public static void SetupSlider(ScrollView scrollPanel)
        {

            List<Scroller> scrollers = scrollPanel.Query<Scroller>().ToList();
            foreach (Scroller scroller in scrollers)
            {
                scroller.Q<VisualElement>("unity-slider").focusable = false;
                scroller.Q<VisualElement>("unity-slider").Q<VisualElement>().focusable = false;
            }
        }

        public static void PlayFadeInAnimation(VisualElement element, float duration)
        {
            element.style.opacity = 0; // Start fully transparent

            DOTween.To(
                () => element.style.opacity.value,
                opacity => element.style.opacity = opacity,
                1f, // Target full opacity
                duration
            ).SetEase(Ease.InOutQuad);
        }

        public static void PlayFadeOutAnimation(VisualElement element, float duration)
        {
            element.style.opacity = 1; // Ensure starting from fully visible

            DOTween.To(
                () => element.style.opacity.value,
                opacity => element.style.opacity = opacity,
                0f, // Target fully transparent
                duration
            ).SetEase(Ease.InOutQuad);
        }
    }
}
