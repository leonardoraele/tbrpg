using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Raele.Util
{
    public class IngameConsole
    {
        public struct IMGUILogEntry
        {
            public string Text;
        }

        public struct HandleLogEntry
        {
            public string Text;
            public Vector3 Position;
            public Color Color;
        }

        public static IngameConsole Instance
            => s_instance = s_instance ?? new IngameConsole();

        private static IngameConsole s_instance;

        private SortedDictionary<float, IMGUILogEntry> imGuiLogs = new SortedDictionary<float, IMGUILogEntry>();
        private SortedDictionary<float, IEnumerable<HandleLogEntry>> handleLogs = new SortedDictionary<float, IEnumerable<HandleLogEntry>>();

        private IngameConsole()
            => UnityEventListener.Instance.OnGUIEvent += this.OnGUI;

        private void OnGUI()
        {
            this.FlushLogs();

            this.imGuiLogs
                .Values
                .Select(log => log.Text)
                .ForEach((text, i) => GUI.Label(new Rect(8, 8 + 16 * i, Screen.width, 16), text));
            
            this.handleLogs
                .Values
                .Flatten()
                .ForEach(log =>
                {
                    Handles.color = log.Color;
                    Handles.Label(log.Position, log.Text);
                });
        }

        public void Log(string text, float duration = -1.0f)
            => this.imGuiLogs[Time.time + duration] = new IMGUILogEntry() { Text = text }; // TODO Do the same logic as the handles log

        public void Log(string text, Vector3 position, float duration = 0.0f, Color color = default)
            => this.handleLogs[Time.time + duration] = new HandleLogEntry()
                { Text = text, Position = Camera.main.WorldToScreenPoint(position).Multiply(new Vector3(1, -1, 1)) + Vector3.up * Screen.height, Color = color }
                .As(out HandleLogEntry newEntry)
                .Then(this.handleLogs.TryGetValue(Time.time + duration, out IEnumerable<HandleLogEntry> logs))
                ? logs.Plus(newEntry)
                : newEntry.ToSingletonCollection();

        /**
         * Removes obsolete elements from the list.
         */
        private void FlushLogs()
        {
            try
            {
                for (float key = this.imGuiLogs.Keys.First(); key < Time.time; key = this.imGuiLogs.Keys.First())
                    this.imGuiLogs.Remove(key);
            } catch {}

            try
            {
                for (float key = this.handleLogs.Keys.First(); key < Time.time; key = this.handleLogs.Keys.First())
                    this.handleLogs.Remove(key);
            } catch {}
        }
    }
}
