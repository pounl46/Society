using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 엔딩 크레딧 항목 하나. Role을 비워두면 이름 한 줄만 표시됨(구분 타이틀 없이).
    /// </summary>
    [System.Serializable]
    public class CreditEntry
    {
        [Tooltip("예: '기획', '아트'. 비워두면 이 줄은 생략되고 Name만 표시됨")]
        public string role;

        [Tooltip("예: '홍길동'")]
        public string name;
    }

    /// <summary>
    /// 크레딧 목록(Entries)을 순서대로 이어붙여 텍스트를 만들고,
    /// 그 텍스트를 아래에서 위로 자동 스크롤 시키는 스크립트.
    /// 씬 시작하자마자 바로 올라가기 시작함.
    ///
    /// 세팅:
    /// 1. 이 스크립트를 TMP 텍스트 오브젝트(Text (TMP))에 직접 Add Component
    /// 2. Entries 배열에 크레딧 항목을 순서대로 추가 (Role/Name 입력)
    /// 3. 이 오브젝트의 Rect Transform Anchored Position Y를 화면 아래(예: -1000)로 배치
    /// 4. End Y Position에 다 올라갔다고 볼 위치 입력 (Play로 테스트하며 조정)
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(TMP_Text))]
    public class CreditsScroller : MonoBehaviour
    {
        [Header("크레딧 내용 (순서대로 표시됨)")]
        [SerializeField] private List<CreditEntry> entries = new List<CreditEntry>();
        [Tooltip("각 항목 사이에 띄울 빈 줄 수")]
        [SerializeField] private int blankLinesBetweenEntries = 1;

        [Header("스크롤 설정")]
        [SerializeField] private float scrollSpeed = 50f; // 초당 몇 픽셀 올라갈지
        [SerializeField] private float endYPosition = 3000f; // 여기까지 올라가면 종료 처리

        [Header("종료 후 동작")]
        [SerializeField] private bool loadSceneOnFinish = true;
        [SerializeField] private string nextSceneName = "TitleScene";

        private RectTransform _rect;
        private TMP_Text _text;
        private bool _finished;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _text = GetComponent<TMP_Text>();
            _text.text = BuildCreditsText();
        }

        private string BuildCreditsText()
        {
            var sb = new StringBuilder();
            string blankLines = new string('\n', blankLinesBetweenEntries + 1);

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.role))
                    sb.Append(entry.role).Append('\n');

                sb.Append(entry.name);
                sb.Append(blankLines);
            }

            return sb.ToString();
        }

        private void Update()
        {
            if (_finished || _rect == null) return;

            Vector2 pos = _rect.anchoredPosition;
            pos.y += scrollSpeed * Time.deltaTime;
            _rect.anchoredPosition = pos;

            if (pos.y >= endYPosition)
            {
                _finished = true;

                if (loadSceneOnFinish)
                    SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}