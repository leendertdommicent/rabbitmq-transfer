using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace RabbitMQTransfer
{
    public class QueueSender
    {
        private IConnection _connection;
        private ILogger _logger;

        public QueueSender(IConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public void PushDumpToQueue(string path)
        {
            var dump = File.ReadAllText(path);
            var states = JsonConvert.DeserializeObject<QueueState[]>(dump);
            using (var model = _connection.CreateModel())
            {
                foreach (var state in states)
                {
                    model.QueueDeclare(
                        queue: state.Name,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    foreach (var message in state.Messages)
                    {
                        var props = model.CreateBasicProperties();
                        message.EnrichBasicProperties(props);
                        model.BasicPublish(state.Name, "", props, message.Body);
                    }
                }
            }
        }
    }
}
