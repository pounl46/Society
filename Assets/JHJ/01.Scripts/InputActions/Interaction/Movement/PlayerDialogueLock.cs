using UnityEngine;
using JHJ.Scripts.Interaction.Dialogue;

namespace JHJ.Scripts.Interaction.Movement
{
    /// <summary>
    /// 대화 중일 때 플레이어 무빙 스크립트를 꺼서 못 움직이게 막는 컴포넌트.
    /// 무빙 스크립트 내용을 몰라도 되고, 그 스크립트를 전혀 안 건드림 -
    /// 그냥 컴포넌트 자체를 enabled = false로 꺼버리는 방식.
    ///
    /// 세팅:
    /// 1. 플레이어 오브젝트에 이 스크립트 Add Component
    /// 2. Dialogue Manager 필드에 씬의 DialogueManager 오브젝트 드래그
    /// 3. Scripts To Disable 배열에 "대화 중 멈춰야 하는" 스크립트들 드래그
    ///    (무빙 스크립트, 필요하면 카메라 회전 스크립트 등도 같이 넣으면 됨)
    /// 4. (선택) Rigidbody 쓰는 무빙이면 Player Rigidbody에 연결 -
    ///    대화 시작 순간 남아있는 속도를 0으로 없애서 미끄러지듯 밀리는 것 방지
    /// </summary>
    public class PlayerDialogueLock : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private DialogueManager dialogueManager;

        [Header("대화 중 꺼야 할 스크립트들 (무빙 스크립트 등)")]
        [SerializeField] private MonoBehaviour[] scriptsToDisable;

        [Header("선택 사항 - Rigidbody 쓰는 경우 속도 초기화")]
        [SerializeField] private Rigidbody playerRigidbody;

        private void OnEnable()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("[PlayerDialogueLock] DialogueManager가 연결되지 않았습니다.", this);
                return;
            }
            dialogueManager.OnDialogueStarted += HandleDialogueStarted;
            dialogueManager.OnDialogueEnded += HandleDialogueEnded;
        }

        private void OnDisable()
        {
            if (dialogueManager == null) return;
            dialogueManager.OnDialogueStarted -= HandleDialogueStarted;
            dialogueManager.OnDialogueEnded -= HandleDialogueEnded;
        }

        private void HandleDialogueStarted()
        {
            SetScriptsEnabled(false);

            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void HandleDialogueEnded()
        {
            SetScriptsEnabled(true);
        }

        private void SetScriptsEnabled(bool isEnabled)
        {
            if (scriptsToDisable == null) return;

            foreach (var script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = isEnabled;
            }
        }
    }
}