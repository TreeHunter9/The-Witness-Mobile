using System.Collections.Generic;
using System.Threading.Tasks;

public class ConditionsOfFinish
{
    private List<Condition> _listOfConditions = new List<Condition>();
    private bool _isGood = true;

    public void ListAdd(Condition value) => _listOfConditions.Add(value);

    public bool CheckConditions()
    {
        _isGood = true;
        List<Task> tasks = new List<Task>(_listOfConditions.Count);
        foreach (var element in _listOfConditions)
        {
            tasks.Add(Check(element));
        }
        Task.WaitAll(tasks.ToArray());

        return _isGood;
    }

    private async Task Check(Condition condition)
    {
        if (condition.CheckCondition() == false)
        {
            condition.Mistake();
            _isGood = false;
        }
    }
}
