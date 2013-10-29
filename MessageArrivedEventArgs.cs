using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class MessageArrivedEventArgs : EventArgs
    {
        private string message;

        public string Message
        {
            get
            {
                return message;
            }
        }

        public MessageArrivedEventArgs()
        {
            message = "No message sent.";
        }

        public MessageArrivedEventArgs(string newMessage)
        {
            message = newMessage;
        }
    }
}