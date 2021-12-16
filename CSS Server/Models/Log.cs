using System;

namespace CSS_Server.Models
{
    public class Log
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
    }
}
