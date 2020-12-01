using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.IO;
using System.Text;

namespace IoTHubDeviceSimulator.Services
{
    /// <summary>
    /// Emits log events as action.
    /// </summary>
    public class ActionSink : ILogEventSink
    {
        private readonly ITextFormatter _formatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{Exception}");

        /// <summary>
        /// The event, that is fired, when a new log message arrives.
        /// </summary>
        public static event Action<string> OnLog;

        public void Emit(LogEvent logEvent)
        {
            var buffer = new StringWriter(new StringBuilder(256));
            _formatter.Format(logEvent, buffer);
            var message = buffer.ToString();

            OnLog?.Invoke(message);
        }
    }
}
