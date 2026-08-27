using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 씬 전환 시 화면을 검은색으로 페이드 인/아웃 시키는 매니저.
    /// 씬이 바뀌어도 유지되도록 DontDestroyOnLoad 처리됨 (싱글톤).
    ///
    /// 세팅:
    /// 1. 빈 오브젝트(SceneFader)에 이 스크립트 Add Component
    /// 2. 그 자식으로 Canvas(Sort Order를 크게, 예: 999) -> 전체 화면을 덮는 검은 Image 생성
    /// 3. 그 Image에 CanvasGroup 컴포넌트 추가
    /// 4. Fade Canvas Group 필드에 그 CanvasGroup 드래그
    /// 5. 씬 최초 시작 시 알파값 1(완전히 까만 상태)로 시작해서 FadeIn 해주는 게 자연스러움
    ///    -> 필요하면 Start()에서 자동으로 FadeIn 하도록 fadeInOnStart 체크
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance { get; private set; }

        [Header("참조")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("설정")]
        [SerializeField] private float fadeDuration = 0.6f;
        [Tooltip("씬이 처음 시작될 때 검은 화면에서 자동으로 밝아지게 할지")]
        [SerializeField] private bool fadeInOnStart = true;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (fadeInOnStart && fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                StartCoroutine(FadeRoutine(1f, 0f, null));
            }
        }

        /// <summary>검은 화면으로 덮은 뒤 지정한 씬을 로드하고 다시 밝아짐</summary>
        public void FadeToScene(string sceneName)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeOutLoadFadeIn(sceneName));
        }

        private IEnumerator FadeOutLoadFadeIn(string sceneName)
        {
            yield return FadeRoutine(0f, 1f, null); // 화면 어둡게

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
            while (load != null && !load.isDone)
                yield return null;

            yield return FadeRoutine(1f, 0f, null); // 다시 밝게
        }

        private IEnumerator FadeRoutine(float from, float to, System.Action onComplete)
        {
            if (fadeCanvasGroup == null)
            {
                onComplete?.Invoke();
                yield break;
            }

            fadeCanvasGroup.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = to;
            fadeCanvasGroup.blocksRaycasts = to > 0.99f; // 다 밝아졌으면 클릭 막지 않음

            onComplete?.Invoke();
        }
    }
}