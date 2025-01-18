using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Utility.Timer
{
    public class EventTimer
    {
        /*
         * By Kenjiro Mai
         * 2024
         */

        /// <summary>
        /// Listeners that will be invoked when the timer is specified time away
        /// from start or end
        /// </summary>
        public class OffsetListener
        {
            public OffsetListener(float offsetSeconds, OffsetReferencePoint referencePoint, float endTimeSeconds, Action listener)
            {
                if (referencePoint == OffsetReferencePoint.FromEnd && offsetSeconds <= 0f)
                {
                    Debug.LogError($"Tried to set offset listener to end of timer; use a LapListener instead.");
                    return;
                }
                
                EndTime = endTimeSeconds;
                ReferencePoint = referencePoint;
                TimeOffsetSeconds = offsetSeconds;
                CalculateCallTime();
                Listener = listener;
            }

            ~OffsetListener()
            {
                // Unsubscribe all listeners
                Listener = null;
            }

            public enum OffsetReferencePoint
            {
                FromStart,
                FromEnd
            }

            public Action Listener;

            /// <summary>
            /// The amount of time away from the start or end
            /// </summary>
            public float TimeOffsetSeconds { get; private set; }

            public OffsetReferencePoint ReferencePoint { get; private set; }

            /// <summary>
            /// When the timer will reset or stop
            /// </summary>
            public float EndTime { get; private set; }

            /// <summary>
            /// When the listener will be notified
            /// </summary>
            public float CallTime { get; private set; }

            // Prevents listener from being called repeatedly
            private bool listenerCalled = false;

            /// <summary>
            /// Checks if the elapsed time exceeds the CallTime. If so, invokes listener once
            /// </summary>
            /// <param name="elapsedTime">Current elapsed time</param>
            public void Poll(float elapsedTime)
            {
                // TODO update this to support call times at 0 (beginning)
                
                if (!listenerCalled && (elapsedTime >= CallTime && elapsedTime < EndTime))
                {
                    listenerCalled = true;
                    Listener?.Invoke();
                }

                if (listenerCalled && elapsedTime >= EndTime)
                {
                    listenerCalled = false;
                }
            }

            /// <summary>
            /// Sets ending time, i.e., the maximum elapsed time of the timer
            /// </summary>
            public void SetEndTime(float newEndTime)
            {
                EndTime = newEndTime;
                CalculateCallTime();
            }

            private void CalculateCallTime()
            {
                CallTime = ReferencePoint == OffsetReferencePoint.FromStart ? TimeOffsetSeconds : EndTime - TimeOffsetSeconds;
            }
        }

        /// <summary>
        /// The length of time of this timer's lap
        /// </summary>
        public float LapTime { get; private set; }

        /// <summary>
        /// If true, the timer will automatically reset to 0 seconds when it meets its lap time
        /// </summary>
        public bool Continuous { get; set; }

        /// <summary>
        /// Elapsed seconds
        /// </summary>
        public float ElapsedSeconds { get; private set; }
        public bool Paused { get; private set; }
        private Action lapListeners;
        private Action<float> pollOffsetListeners;
        private List<OffsetListener> offsetListeners = new();

        public EventTimer(float lapTime, bool continuous)
        {
            if (TimerManager.Instance == null)
            {
                Debug.LogError("No instance of TimeManager detected");
                return;
            }
            LapTime = lapTime;
            Continuous = continuous;
            TimerManager.Instance.UpdateTimer += AddTime;
        }

        /// <summary>
        /// Sets the lap time for this timer. Automatically pauses and resets timer.
        /// </summary>
        /// <param name="time">New lap time</param>
        public void SetLapTime(float time)
        {
            LapTime = time;
            offsetListeners.ForEach(x => x.SetEndTime(time));
            StopTimer();
            StartTimer();
        }

        /// <summary>
        /// Resumes or starts the timer
        /// </summary>
        public void StartTimer()
        {
            Paused = false;
        }

        /// <summary>
        /// Pauses the timer and resets elapased time to 0
        /// </summary>
        public void StopTimer()
        {
            ElapsedSeconds = 0;
            Paused = true;
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        public void PauseTimer()
        {
            Paused = true;
        }

        /// <summary>
        /// Runs StopTimer() then StartTimer()
        /// </summary>
        public void ResetTimer()
        {
            StopTimer();
            StartTimer();
        }

        /// <summary>
        /// Add a listener. Will be called whenever the timer completes a lap
        /// </summary>
        /// <param name="listener">Listener to add</param>
        public void AddLapListener(Action listener)
        {
            lapListeners += listener;
        }

        /// <summary>
        /// Unsubscribe all listeners
        /// </summary>
        /// <returns>List of old delegates</returns>
        public List<Delegate> ClearLapListeners()
        {
            var oldListeners = lapListeners?.GetInvocationList().ToList();
            lapListeners = () => { };
            return oldListeners;
        }

        public List<OffsetListener> ClearOffsetListeners()
        {
            var listeners = offsetListeners?.ToList();
            pollOffsetListeners = null;
            offsetListeners?.Clear();
            return listeners;
        }

        /// <summary>
        /// Changes the current elapsed time by given seconds
        /// </summary>
        /// <param name="seconds">How much to change elapsed time by</param>
        public void ChangeElapsedTime(float seconds)
        {
            ElapsedSeconds += seconds;
            pollOffsetListeners?.Invoke(ElapsedSeconds);

            if (ElapsedSeconds >= LapTime)
            {
                ElapsedSeconds = 0f;
                lapListeners?.Invoke();
            }

            if (ElapsedSeconds < 0f)
            {
                ElapsedSeconds = 0f;
            }
        }

        /// <summary>
        /// Add a listener that will be invoked when the timer is some time away from the start or end
        /// </summary>
        /// <param name="listener">Listener to add</param>
        /// <param name="offsetTime">Time in seconds away from reference point</param>
        /// <param name="offsetReferencePoint">Should the call time be calculated from the start or end?</param>
        public void AddOffsetListener(Action listener, float offsetTime, OffsetListener.OffsetReferencePoint offsetReferencePoint)
        {
            var newListener = new OffsetListener(offsetTime, offsetReferencePoint, LapTime, listener);
            pollOffsetListeners += newListener.Poll;
            offsetListeners.Add(newListener);
        }

        /// <summary>
        /// Destroys this timer
        /// </summary>
        public void Destroy()
        {
            pollOffsetListeners = null;
            lapListeners = null;
            offsetListeners.Clear();
            TimerManager.Instance.UpdateTimer -= AddTime;
            ClearLapListeners();
        }

        private void AddTime(object sender, TimerUpdateArgs args)
        {
            if (Paused) return;

            ElapsedSeconds += args.DeltaTime;
            pollOffsetListeners?.Invoke(ElapsedSeconds);

            if (ElapsedSeconds >= LapTime)
            {
                if (!Continuous)
                {
                    PauseTimer();
                }
                ElapsedSeconds = 0f;
                lapListeners?.Invoke();
            }
        }

        ~EventTimer()
        {
            Destroy();
        }
    }
}
