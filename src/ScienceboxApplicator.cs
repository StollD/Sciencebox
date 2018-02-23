using System;
using UnityEngine;

namespace Sciencebox
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ScienceboxApplicator : MonoBehaviour
    {
        /// <summary>
        /// Whether the savegame that was loaded is in Sciencebox mode
        /// </summary>
        public static Boolean isActive = false;
        
        /// <summary>
        /// Unlocks all parts and functions
        /// </summary>
        void Start()
        {
            if (isActive)
            {
                ResearchAndDevelopment.Instance.CheatTechnology();
                isActive = false;
            }
        }
    }
}