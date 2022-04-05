using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Loggers
{
    public class LogFormatter : ITextFormatter
    {
        public class FormattedLog
        {
            public DateTimeOffset Timestamp { get; set; }
            public string Level { get; set; }
            public string Trace { get; set; }
            public string RequestPath { get; set; }
            public string CorrelationId { get; set; }
            public string IdentificationMessage { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            LogEventPropertyValue dictionaryValue;
            var outputLog = new FormattedLog()
            {
                Timestamp = logEvent.Timestamp,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                Trace = logEvent.Properties.TryGetValue("SourceContext", out dictionaryValue) ? dictionaryValue.ToString().Replace("\"", "") : null,
                RequestPath = logEvent.Properties.TryGetValue("RequestPath", out dictionaryValue) ? dictionaryValue.ToString().Replace("\"", "") : null,
                CorrelationId = logEvent.Properties.TryGetValue("RequestCorrelationId", out dictionaryValue) ? dictionaryValue.ToString().Replace("\"", "") : null,
                IdentificationMessage = logEvent.Properties.TryGetValue("IdentificationMessage", out dictionaryValue) ? dictionaryValue.ToString().Replace("\"", "") : null,
                
                // seems is better to use ToString https://github.com/RehanSaeed/Serilog.Exceptions
                
                Exception = logEvent.Exception?.ToString()
            };

            var nullSanitizerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

            };
            output.Write("{0}", JsonConvert.SerializeObject(outputLog, Formatting.None, nullSanitizerSettings));
            output.Write(Environment.NewLine);
        }
    }
}
