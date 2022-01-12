using System;
using UnityEngine;

public class EndBlockTrigger : MonoBehaviour
{
    public event Action<Vector3> onFinishLevel;

    private ConditionsOfFinish _conditionsOfFinish;
    private LevelsData _levelsData;

    public void Init(ConditionsOfFinish conditionsOfFinish, LevelsData levelsData)
    {
        _conditionsOfFinish = conditionsOfFinish;
        _levelsData = levelsData;
    }

    public bool CheckConditions()
    {
        if (_conditionsOfFinish != null)
        {
            if (_conditionsOfFinish.CheckConditions() == false)
            {
                return false;
            }
        }
        _levelsData.LevelComplete();
        
        return true;
    }
}
