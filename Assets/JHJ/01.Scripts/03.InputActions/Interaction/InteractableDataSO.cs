using UnityEngine;

namespace JHJ.Scripts.Interaction.Interaction
{
    /// <summary>
    /// 상호작용 오브젝트의 데이터를 정의하는 SO.
    /// 코드 수정 없이 에디터에서 프롬프트 문구, 감지 범위 등을 조정할 수 있게 하기 위함.
    /// 우클릭 -> Create -> Game -> Interaction -> Interactable Data 로 생성.
    /// </summary>
    [CreateAssetMenu(fileName = "New Interactable Data", menuName = "Game/Interaction/Interactable Data")]
    public class InteractableDataSO : ScriptableObject
    {
        [Header("표시 정보")]
        [SerializeField] private string interactableName = "Object";
        [SerializeField, TextArea] private string interactionPrompt = "F - 상호작용";

        [Header("상호작용 설정")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private bool requireLineOfSight = true; // TODO: LOS 체크는 추후 PlayerInteractor에 연결

        public string InteractableName => interactableName;
        public string InteractionPrompt => interactionPrompt;
        public float InteractionRange => interactionRange;
        public bool RequireLineOfSight => requireLineOfSight;
    }
}