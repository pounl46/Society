using UnityEngine;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 타이틀 화면 버튼 3개(시작하기 / 설정 / 게임 종료) 처리.
    /// 버튼의 OnClick()에서 이 스크립트의 메서드를 각각 연결하면 됨.
    /// </summary>
    public class TitleMenuController : MonoBehaviour
    {
        [Header("시작할 게임 씬 이름 (Build Settings에 등록된 이름과 정확히 일치해야 함)")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("참조")]
        [SerializeField] private OptionsPanelController optionsPanel;

        /// <summary>"시작하기" 버튼 OnClick에 연결</summary>
        public void OnClickStart()
        {
            if (SceneFader.Instance != null)
                SceneFader.Instance.FadeToScene(gameSceneName);
            else
                Debug.LogWarning("[TitleMenuController] SceneFader가 씬에 없습니다. 즉시 전환은 안 되니 SceneFader 오브젝트를 추가하세요.");
        }

        /// <summary>"설정" 버튼 OnClick에 연결</summary>
        public void OnClickOptions()
        {
            if (optionsPanel != null)
                optionsPanel.Open();
            else
                Debug.LogWarning("[TitleMenuController] OptionsPanel이 연결되지 않았습니다.");
        }

        /// <summary>"게임 종료" 버튼 OnClick에 연결</summary>
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