using System;
using UnityEngine;
using ZFreeHome.Tasks;

namespace Tasks
{
    [Serializable]
    public class ExebitionTask : TaskComponent
    {
        [SerializeField] private bool checkSeason;
        [SerializeField] private Season season;
        [SerializeField] private bool checkTimeOfDay;
        [SerializeField] private TimeOfDay timeOfDay;
        [SerializeField] private bool checkLocation;
        [SerializeField] private Location location;
        private ExebitionManager _exebitionManager;
        
        private void Start()
        {
            _exebitionManager = GetComponentInParent<ExebitionManager>();
        }

        public void Update()
        {
            if ((!checkSeason || _exebitionManager.currentSeason == season) &&
                (!checkLocation || _exebitionManager.currentLocation == location) &&
                (!checkTimeOfDay || _exebitionManager.currentTimeOfDay == timeOfDay)&&
                active)
            {
                Complete();
            }
        }

        public override string GetDescription()
        {
            string disc = "";
            if (checkLocation)
            {
                if (_exebitionManager.currentLocation == location)
                {
                    
                    disc += "<s>Set Location To " + location+"</s>";
                }
                else
                {
                    disc += "Set Location To " + location;
                }
                disc += "\n";
            }
            if (checkSeason)
            {
                if (_exebitionManager.currentSeason ==season)
                {
                    disc += "<s>Set Season To " + season +"</s>";
                }
                else
                {
                    disc += "Set Season To " + season;
                }
                disc += "\n";
            }

            if (checkTimeOfDay)
            {
                if (_exebitionManager.currentTimeOfDay ==timeOfDay)
                {
                    disc += "<s>Set Time of Day To" + timeOfDay +"</s>";
                }
                else
                {
                    disc += "Set Time of Day To " + timeOfDay;
                }
                disc += "\n";
            }
            return disc;
        }
    }
}