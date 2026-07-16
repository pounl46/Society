using System.Collections.Generic;
using UnityEngine;

namespace JHJ.Scripts.Interaction.Dialogue
{
    /// <summary>
    /// NPC 하나의 대사 세트를 담는 SO.
    /// 우클릭 -> Create -> Game -> Dialogue -> Dialogue Data 로 생성.
    /// NPC마다 하나씩 만들어서 DialogueTrigger에 연결하면 됨.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Game/Dialogue/Dialogue Data")]
    public class DialogueDataSO : ScriptableObject
    {
        [SerializeField] private DialogueLineData[] lines;

        public IReadOnlyList<DialogueLineData> Lines => lines;
    }
}