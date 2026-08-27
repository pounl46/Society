using UnityEngine;

namespace JHJ.Scripts.UI.Title
{
    /// <summary>
    /// 설정 패널 열고 닫기만 담당하는 껍데기 스크립트.
    /// 볼륨 슬라이더, 해상도 설정 등 실제 옵션 항목은 나중에
    /// 이 스크립트 안에 필드/메서드로 추가하면 됨 (지금은 열기/닫기만 동작).
    /// </summary>
    public class OptionsPanelController : MonoBehaviour
    {
        [Header("참조")]
        [Tooltip("설정 패널 전체 오브젝트 (켜고 끌 대상)")]
        [SerializeField] private GameObject panelRoot;

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void Update()
        {
            if (panelRoot != null && panelRoot.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        public void Open()
        {
            if (panelRoot != null)
                panelRoot.SetActive(true);
        }

        /// <summary>"닫기" 버튼 OnClick에 연결</summary>
        public void Close()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }
    }
}