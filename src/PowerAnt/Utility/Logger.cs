namespace AntMe.Player.PowerAnt
{
    using System;
    using System.Diagnostics;

    public class Logger
    {
        public void Debug(string message)
        {
            Trace.WriteLine($"{DateTime.Now:HH:mm:ss} [D] {message}");
        }
    }
}
