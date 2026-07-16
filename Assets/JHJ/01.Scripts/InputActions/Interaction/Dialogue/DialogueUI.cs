using System.Collections;
using UnityEngine;
using TMPro; // 레거시 Text 쓸 거면 이 줄 지우고 아래 TMP_Text -> UnityEngine.UI.Text 로 교체

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// DialogueManager의 이벤트만 구독해서 화면에 표시만 하는 순수 UI 스크립트.
    /// 대사가 바뀌면 한 글자씩 타이핑되는 효과로 표시함.
    ///
    /// 좌클릭(Advance) 시:
    /// - 타이핑 중이면: 즉시 전체 텍스트를 다 보여주고, 줄은 넘기지 않음
    /// - 타이핑이 이미 끝났으면: 아무것도 안 하고 매니저가 다음 줄로 넘기게 둠
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private DialogueManager dialogueManager;

        [Header("타이핑 효과")]
        [Tooltip("한 글자당 걸리는 시간(초). 작을수록 빠름")]
        [SerializeField] private float typingInterval = 0.03f;

        private Coroutine _typingCoroutine;
        private string _currentFullLine = "";
        private bool _isTyping;

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (dialogueManager == null) return;
            dialogueManager.OnDialogueStarted += HandleDialogueStarted;
            dialogueManager.OnLineChanged += HandleLineChanged;
            dialogueManager.OnDialogueEnded += HandleDialogueEnded;
            dialogueManager.OnAdvanceRequested += HandleAdvanceRequested;
        }

        private void OnDisable()
        {
            if (dialogueManager == null) return;
            dialogueManager.OnDialogueStarted -= HandleDialogueStarted;
            dialogueManager.OnLineChanged -= HandleLineChanged;
            dialogueManager.OnDialogueEnded -= HandleDialogueEnded;
            dialogueManager.OnAdvanceRequested -= HandleAdvanceRequested;
        }

        private void HandleDialogueStarted()
        {
            if (panelRoot != null)
                panelRoot.SetActive(true);
        }

        private void HandleLineChanged(DialogueLineData lineData)
        {
            if (speakerNameText != null)
                speakerNameText.text = lineData.speakerName;

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _currentFullLine = lineData.line;
            _typingCoroutine = StartCoroutine(TypeLineRoutine(_currentFullLine));
        }

        private void HandleDialogueEnded()
        {
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _isTyping = false;

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        /// <summary>
        /// DialogueManager가 좌클릭 들어올 때마다 물어보는 콜백.
        /// 타이핑 중이면 스킵 처리하고 true(소비함) 반환, 아니면 false 반환.
        /// </summary>
        private bool HandleAdvanceRequested()
        {
            if (_isTyping)
            {
                SkipTyping();
                return true;
            }
            return false;
        }

        private IEnumerator TypeLineRoutine(string fullLine)
        {
            _isTyping = true;

            if (dialogueText != null)
                dialogueText.text = "";

            for (int i = 0; i < fullLine.Length; i++)
            {
                if (dialogueText != null)
                    dialogueText.text = fullLine.Substring(0, i + 1);

                yield return new WaitForSeconds(typingInterval);
            }

            _isTyping = false;
            _typingCoroutine = null;
        }

        private void SkipTyping()
        {
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            if (dialogueText != null)
                dialogueText.text = _currentFullLine;

            _isTyping = false;
            _typingCoroutine = null;
        }
    }
}