using UnityEngine;
using UnityEngine.SceneManagement;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 게임 오버 패널. "다시하기" / "나가기" 버튼 처리.
    /// 다시하기는 씬 이름을 직접 입력받아 그 씬을 로드함.
    /// </summary>
    public class GameOverController : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private GameObject panelRoot;

        [Header("다시하기 시 로드할 씬 이름 (Build Settings에 등록된 이름과 정확히 일치해야 함)")]
        [SerializeField] private string retrySceneName = "GameScene";

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        /// <summary>게임 오버 상황이 발생했을 때 아무 스크립트에서나 호출</summary>
        public void Show()
        {
            if (panelRoot != null)
                panelRoot.SetActive(true);
        }

        /// <summary>"다시하기" 버튼 OnClick에 연결</summary>
        public void OnClickRetry()
        {
            SceneManager.LoadScene(retrySceneName);
        }

        /// <summary>"나가기" 버튼 OnClick에 연결</summary>
        public void OnClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}