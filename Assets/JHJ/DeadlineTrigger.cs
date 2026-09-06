using UnityEngine;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 바닥 아래 등에 설치하는 데드라인. 플레이어가 여기 닿으면 게임 오버 패널을 띄움.
    ///
    /// 세팅:
    /// 1. 빈 오브젝트 생성 -> 원하는 위치(맵 아래쪽)에 배치
    /// 2. Box Collider(또는 원하는 모양) Add Component -> Is Trigger 체크 ON
    ///    -> Size를 넓게 잡아서 맵 전체를 덮게
    /// 3. 이 스크립트 Add Component
    /// 4. Game Over Controller 필드에 게임오버 패널 담당 오브젝트 드래그
    /// 5. 플레이어 오브젝트의 Tag가 "Player"인지 확인
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DeadlineTrigger : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private GameOverController gameOverController;

        private void Reset()
        {
            // 콜라이더를 처음 추가할 때 자동으로 트리거로 세팅해줌 (실수 방지)
            var col = GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            if (gameOverController != null)
                gameOverController.Show();
            else
                Debug.LogWarning("[DeadlineTrigger] GameOverController가 연결되지 않았습니다.", this);
        }
    }
}