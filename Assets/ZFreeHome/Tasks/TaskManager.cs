using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZFreeHome.Tasks;

namespace Tasks
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private List<TaskComponent> tasks;
        [SerializeField] private List<TaskComponent> outsideTasks;
        
        [SerializeField] private TextMeshPro wallText;
        [SerializeField] private TextMeshPro outsideText;

        [SerializeField] private bool moveOutside;
        
        
        private string completedTasksText;
        [SerializeField] private TaskComponent activeTask;
        [SerializeField] private ExebitionManager _exebitionManager;
        [SerializeField] private CompletedAllTasksTask completedAllTasksTask;
        
        [SerializeField] private float fps;
        public int taskNumber;
        public bool started;

        [UsedImplicitly]
        public void EmergencyButton(int gestureID )
        {
            switch (gestureID)    
            {
                case  0:
                    _exebitionManager.NextLight();
                    break;
                case 1:
                    _exebitionManager.NextLocation();
                    break;
                case 2:
                    _exebitionManager.NextSeason();
                    break; 
            }
        }
        private void Start()
        {
            completedAllTasksTask = gameObject.AddComponent<CompletedAllTasksTask>();
            ResetTasks();
        }

        public void ResetTasks()
        {
            moveOutside = false;
            foreach (TaskComponent task in tasks)
            {
                task.completed = false;
            }
            foreach (TaskComponent task in outsideTasks)
            {
                task.completed = false;
            }
            if (tasks == null) return;
            ActivateNextTask(tasks);
            taskNumber = 0;
        }

        public void Update()
        {
            if (!started) { return;}
            UpdateTaskList();
        }

        private void UpdateTaskList()
        {
            if (moveOutside)
            {
                foreach (var task in outsideTasks.Where(task => task.completed && task.active))
                {
                    task.Deactivate();
                    ActivateNextTask(outsideTasks);
                }
            }
            else
            {
                foreach (var task in tasks.Where(task => task.completed && task.active))
                {
                    task.Deactivate();
                    ActivateNextTask(tasks);
                }
            }

            if (activeTask == completedAllTasksTask )
            {
                ActivateNextTask(outsideTasks);
                moveOutside = true;
            }
            RepaintWallText();
        }
        private void RepaintWallText()
        {
            if (moveOutside)
            {
                wallText.text = "Go Outside";
                outsideText.text = activeTask.GetDescription();
                return;
            }
            wallText.text =  activeTask.GetDescription();
            outsideText.text = "";
        }
        private void ActivateNextTask(List<TaskComponent> taskList)
        {
            foreach (var task in taskList.Where(task => !task.completed))
            {
                task.Activate();
                activeTask = task;
                taskNumber++;
                return;
            }
            activeTask = completedAllTasksTask;
        } 
    }
}