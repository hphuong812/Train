using System;
using System.Collections;
using ADN.Meta.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IsoMatrix.Scripts.UI
{
    public class ScreenLoadingController : MonoBehaviour, IEventListener<ScreenEvent>
    {
        public const string PREFAB_KEY = "prefab_loading_screen";
        public static ScreenLoadingController Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindObjectOfType<ScreenLoadingController>();

                if (s_Instance != null)
                    return s_Instance;

                Create ();

                return s_Instance;
            }
        }

        public CanvasGroup faderCanvasGroup;
        public Slider slider;
        public float fadeDuration = 1f;

        protected static ScreenLoadingController s_Instance;
        public UnityEvent onFadeSceneIn;
        public UnityEvent onFadeSceneOut;

        public bool IsFading { get; set; }

        public static void Create ()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(PREFAB_KEY);
            var controllerPrefab = handle.WaitForCompletion();
            var go = Instantiate (controllerPrefab);
            var controller = go.GetComponent<ScreenLoadingController>();
            s_Instance = controller;
        }

        void Awake ()
        {
            if (Instance != this)
            {
                Destroy (gameObject);
                return;
            }

            DontDestroyOnLoad (gameObject);
            EventManager.Subscribe(this);
        }

        private void Start()
        {

        }

        protected IEnumerator Fade(float finalAlpha)
        {
            faderCanvasGroup.blocksRaycasts = true;
            float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
            {
                faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                    fadeSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            faderCanvasGroup.alpha = finalAlpha;
        }

        public static IEnumerator FadeSceneOut ()
        {
            Instance.faderCanvasGroup.blocksRaycasts = false;
            Instance.IsFading = true;
            Instance.onFadeSceneOut.Invoke();
            yield return null;
        }

        public static IEnumerator FadeSceneIn ()
        {
            Instance.faderCanvasGroup.blocksRaycasts = true;
            Instance.faderCanvasGroup.gameObject.SetActive (true);
            Instance.IsFading = true;
            Instance.onFadeSceneIn.Invoke();
            yield return null;
        }

        public static void SetProgress(float progress)
        {
        }

        public void OnEventTriggered(ScreenEvent e)
        {
            switch (e.type)
            {
                case ScreenEventType.ScreenIn :
                    StartCoroutine(FadeSceneIn());
                    break;
                case ScreenEventType.ScreenOut :
                    StartCoroutine(FadeSceneOut());
                    break;
            }
        }
    }
}
