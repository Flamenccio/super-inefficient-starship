using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Utility.Timer
{
    public class TimerManager : MonoBehaviour
    {
        /*
         * By Kenjiro Mai
         * 2024
         */

        public static TimerManager Instance;

        public event EventHandler<TimerUpdateArgs> UpdateTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            OnTimeUpdate(new(Time.deltaTime));
        }

        protected virtual void OnTimeUpdate(TimerUpdateArgs args)
        {
            UpdateTimer?.Invoke(this, args);
        }

        /// <summary>
        /// Removes all linked timers. Warning! This is dangerous!
        /// </summary>
        /// <returns>
        /// List of all removed Delegates
        /// </returns>
        public List<Delegate> RemoveAllTimers()
        {
            var invocationList = UpdateTimer.GetInvocationList().ToList();
            UpdateTimer = null;

            return invocationList;
        }
    }

    public class TimerUpdateArgs : EventArgs
    {
        public TimerUpdateArgs(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        public float DeltaTime { get; private set; }
    }
}