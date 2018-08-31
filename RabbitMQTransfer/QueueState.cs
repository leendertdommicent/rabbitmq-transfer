using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;

namespace RabbitMQTransfer
{
    public class QueueState
    {
        public string Name { get; set; }

        public List<Message> Messages { get; set; }

        public QueueState()
        {
            Messages = new List<Message>();
        }
    }
}
