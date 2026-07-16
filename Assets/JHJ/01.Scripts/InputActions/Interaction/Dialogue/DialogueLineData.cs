using UnityEngine;

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// 대사 한 줄의 데이터. MonoBehaviour도 SO도 아니고
    /// DialogueDataSO 안에 배열로 들어가는 순수 데이터 클래스.
    /// </summary>
    [System.Serializable]
    public class DialogueLineData
    {
        public string speakerName;

        [TextArea(2, 5)]
        public string line;
    }
}