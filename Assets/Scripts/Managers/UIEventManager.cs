using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Flamenccio.HUD
{
    public struct UIMessage
    {
        public object Value;
        public Type ValueType;
    }

    /// <summary>
    /// Allows more decoupled communication with UI controllers
    /// </summary>
    public static class UIEventManager
    {
        public static bool Instanced { get => true; }

        /// <summary>
        /// Tells the UI to display the weapon selection screen with given weapons
        /// <para>
        /// <b>UIMessage</b> contains the list of weapons to display
        /// </para>
        /// </summary>
        public static Action<UIMessage> DisplayWeapons { get; set; }

        /// <summary>
        /// Tells the UI to display the normal game HUD
        /// </summary>
        public static Action DisplayGameHUD { get; set; }

        /// <summary>
        /// Remove all subscriptions from all events.
        /// </summary>
        public static void ClearAllEvents()
        {
            var fields = typeof(UIEventManager).GetProperties(BindingFlags.Static | BindingFlags.Public);

            fields
                .Where(f => f.PropertyType == typeof(Action<UIMessage>) || f.PropertyType == typeof(Action))
                .ToList()
                .ForEach(f => {
                    if (f.PropertyType == typeof(Action<UIMessage>))
                    {
                        f.SetValue(null, (Action<UIMessage>)((_) => { }));
                    }
                    if (f.PropertyType == typeof(Action))
                    {
                        f.SetValue(null, new Action(() => { }));
                    }
                    });
        }

        /// <summary>
        /// Creates a UIMessage with the given parameters
        /// </summary>
        public static UIMessage CreateUIMessage(object value, Type valueType)
        {
            return new()
            {
                Value = value,
                ValueType = valueType
            };
        }
    }
}