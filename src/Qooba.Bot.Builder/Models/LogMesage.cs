namespace Qooba.Bot.Builder.Models
{
    public class LogMessage
    {
        string value;
        public LogMessage(string value)
        {
            this.value = value;
        }

        public string Value => this.value;

        public static explicit operator LogMessage(string v) => new LogMessage(v);
    }
}