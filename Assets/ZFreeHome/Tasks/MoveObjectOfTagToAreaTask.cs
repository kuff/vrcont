using System;
using UnityEngine;

namespace Tasks
{
    public class MoveObjectOfTagToAreaTask : TaskComponent
    {
        [SerializeField] private string objectTag;
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(objectTag) && active)
            {
                completed = true;
            }
        }
    }
}
    