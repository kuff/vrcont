using System.Collections.Generic;
using System.Linq;
using Tasks;
using UnityEngine;

public class MultipleTasks : TaskComponent
{
    [SerializeField] private List<TaskComponent> tasks;

    private void Update()
    {
        if(!active) return;
        foreach (var task in tasks)
        {
            task.active = true;
        }
        bool tryComplete = true;
        foreach (var task in tasks.Where(task => !task.completed))
        {
            tryComplete = false;
        }
        completed = tryComplete;
    }

    public override string GetDescription()
    {
        string disc = "";
        foreach (TaskComponent taskComponent in tasks)
        {
            disc += taskComponent.GetDescription();
        }

        return disc;
    }

}