using System;
using UnityEngine;
using JHJ.Scripts.Interaction.Interaction;

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// 대화 상태를 관리하는 씬 유일 매니저 (싱글톤).
    /// UI는 이 매니저의 이벤트만 구독하면 되고, 매니저는 UI를 전혀 모름 (완전 분리).
    ///
    /// CompletedCount: 지금까지 완료된 대화 개수 (순서 진행도).
    /// DialogueTrigger가 자신의 RequiredOrder와 이 값을 비교해서
    /// "내 차례가 됐는지"를 스스로 판단하는 데 사용함.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        public static bool IsDialogueActive { get; private set; }

        /// <summary>지금까지 완료된 대화 개수. 0부터 시작 (아직 아무도 안 끝냄).</summary>
        public static int CompletedCount { get; private set; }

        [Header("참조")]
        [Tooltip("좌클릭 입력을 받아올 Input Reader SO (Game/Input/Dialogue Input Reader 로 생성)")]
        [SerializeField] private DialogueInputReader inputReader;

        public event Action<DialogueLineData> OnLineChanged;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Func<bool> OnAdvanceRequested;

        private DialogueDataSO _currentDialogue;
        private int _lineIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.OnAdvancePressed += HandleAdvancePressed;
            else
                Debug.LogWarning("[DialogueManager] DialogueInputReader가 연결되지 않았습니다.", this);
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnAdvancePressed -= HandleAdvancePressed;
        }

        public void StartDialogue(DialogueDataSO data)
        {
            if (data == null || data.Lines.Count == 0) return;
            if (IsDialogueActive) return;

            _currentDialogue = data;
            _lineIndex = 0;
            IsDialogueActive = true;

            OnDialogueStarted?.Invoke();
            OnLineChanged?.Invoke(_currentDialogue.Lines[_lineIndex]);
        }

        private void HandleAdvancePressed()
        {
            if (!IsDialogueActive) return;

            bool consumedByTyping = OnAdvanceRequested?.Invoke() ?? false;
            if (consumedByTyping) return;

            AdvanceLine();
        }

        private void AdvanceLine()
        {
            _lineIndex++;
            if (_lineIndex >= _currentDialogue.Lines.Count)
            {
                EndDialogue();
                return;
            }
            OnLineChanged?.Invoke(_currentDialogue.Lines[_lineIndex]);
        }

        private void EndDialogue()
        {
            IsDialogueActive = false;
            _currentDialogue = null;
            _lineIndex = 0;

            CompletedCount++;
            Debug.Log($"[DialogueManager] 대화 완료. CompletedCount={CompletedCount}");

            OnDialogueEnded?.Invoke();
        }

        /// <summary>
        /// 씬 재시작이나 테스트용으로 진행도를 초기화하고 싶을 때 호출.
        /// (필요 없으면 안 써도 됨)
        /// </summary>
        public static void ResetProgress()
        {
            CompletedCount = 0;
        }
    }
}