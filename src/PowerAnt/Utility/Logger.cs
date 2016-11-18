namespace AntMe.Player.PowerAnt.Utility
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Simple logger
    /// </summary>
    public class Logger
    {
        public void Debug(string message)
        {
            Trace.WriteLine($"{DateTime.Now:HH:mm:ss} [D] {message}");
        }
    }
}
