

using System;
using UnityEngine;

namespace Tasks
{
    [Serializable]
    public  class TaskComponent : MonoBehaviour,ITask
    {

        public bool completed { get; set; }
        public bool active { get; set; }
        public string taskDescription { get; set; }

        public virtual string GetDescription()
        {
            return taskDescription;
        }

        public void Complete()
        {
            completed = true;
        }

        public virtual void Activate()
        {
            active = true;
        }
        public virtual void Deactivate()
        {
            active = false;
        }
    }
}