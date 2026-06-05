using UnityEngine;

[CreateAssetMenu(fileName = "StageProgression", menuName = "Combat/Stage Progression")]
public sealed class StageProgression : ScriptableObject
{
    [SerializeField] private StageData[] stages;

    public int TotalStages => stages != null ? stages.Length : 0;

    public StageData GetStage(int index)
    {
        if (stages == null || index < 0 || index >= stages.Length)
        {
            return null;
        }

        return stages[index];
    }

    public int GetStageIndex(StageData stage)
    {
        if (stages == null || stage == null)
        {
            return -1;
        }

        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] == stage)
            {
                return i;
            }
        }

        return -1;
    }
}
