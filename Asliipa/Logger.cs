using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Whizz
{
    public enum LogLevel
    {
        /// <summary>
        /// Most detailed logging level, detailing every step
        /// </summary>
        Trace,
        /// <summary>
        /// Supplementary info used for debugging (ex. sdl errors)
        /// </summary>
        Debug,
        /// <summary>
        /// "Heartbeat baseline" logging level, set by default
        /// </summary>
        Info,
        /// <summary>
        /// Easily recoverable errors and exceptions
        /// </summary>
        Warn,
        /// <summary>
        /// Recoverable errors followed by fallback logic or subsystem shutdown
        /// </summary>
        Error,
        /// <summary>
        /// Unrecoverable errors
        /// </summary>
        Fatal
    }

    public enum FilterType
    {
        Exclude,
        Include,
        Until,
        After
    }

    /// <summary>
    /// Primary logging interface, which reports log events to every <see cref="Logger"/>
    /// </summary>
    public class LoggerAgent
    {
        public List<Logger> BoundLoggers = [];
        public string? Group { get; set; }
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Info;

        public void BindTo(Logger logger) => BoundLoggers.Add(logger);

        public LoggerAgent(string? group = null)
        {
            Group = group;
        }

        public void Log(string message, LogLevel? level = null)
        {
            foreach (var logger in BoundLoggers)
                logger.Notify(this, level ?? DefaultLogLevel, message);
        }
    }

    /// <summary>
    /// Device capable of forwarding logging events to a supplied output device (most commonly a console cout).
    /// Does not support messaging directly, use <see cref="LoggerAgent"/> that is bound to this instance instead.
    /// </summary>
    public class Logger
    {
        public TextWriter OutputDevice;
        public bool RichText { get; set; } = true;
        public List<LogLevel> LogFilter { get; set; } = [];
        public FilterType FilterType { get; set; } = FilterType.After;

        protected string GetForeground(Color color) => $"\x1b[38;2;{color.R};{color.G};{color.B}m";
        protected string GetBackground(Color color) => $"\x1b[48;2;{color.R};{color.G};{color.B}m";
        protected const string ColorReset = "\x1b[0m";
        protected const string StandardForeground = $"\x1b[38;2;242;240;229m";
        protected const string StandardBackground = $"\x1b[48;2;69;68;79m";

        protected Color DispatchLogLevelColor(LogLevel level) =>
            level switch
            {
                LogLevel.Trace => Color.FromArgb(134, 129, 136),
                LogLevel.Debug => Color.FromArgb(184, 181, 185),
                LogLevel.Info => Color.FromArgb(75, 128, 202),
                LogLevel.Warn => Color.FromArgb(211, 160, 104),
                LogLevel.Error => Color.FromArgb(128, 73, 58),
                LogLevel.Fatal => Color.FromArgb(180, 82, 82),
                _ => Color.FromArgb(242, 240, 229)
            };

        public LoggerAgent GetAgent(string? group = null)
        {
            var agent = new LoggerAgent(group);
            agent.BindTo(this);
            return agent;
        }

        public void Notify(LoggerAgent agent, LogLevel level, string logMsg)
        {
            if (!FilterPass(level)) return;

            string groupPrefix = agent.Group == null ? "" : $"[{agent.Group}]";
            string logLevelColor = GetForeground(DispatchLogLevelColor(level));
            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            string message;
            if (RichText)
            {
                message = $"{StandardBackground}{StandardForeground}{timestamp} " +
                          $"[{logLevelColor}{level.ToString().ToUpper()}{StandardForeground}]" +
                          $"{groupPrefix} {logMsg}{StandardBackground}";
            } else
            {
                message = $"{timestamp} [{level}]{groupPrefix} {logMsg}";
            }
            
            OutputDevice.WriteLine(message);
        }

        protected bool FilterPass(LogLevel level) => FilterType switch
            {
                FilterType.After => level >= LogFilter.Min(),
                FilterType.Until => level <= LogFilter.Max(),
                FilterType.Include => LogFilter.Contains(level),
                FilterType.Exclude => !LogFilter.Contains(level),
                _ => false,
            };

        public Logger(TextWriter? output = null)
        {
            OutputDevice = output ?? Console.Out;
        }
    }
}
