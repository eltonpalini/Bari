using System;
using System.Collections.Generic;
using System.Text;

namespace Bari.Prova.Domain
{
    public class Message
    {
        public Message() { }
        public Message(string appId, DateTime timeStamp, string id, string text)
        {
            AppId = appId;
            TimeStamp = timeStamp;
            Id = id;
            Text = text
        }

        public string AppId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
