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
        private JsonConverter _converter;

        public QueueSender(IConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
            _converter = new JsonConverter();
        }

        public void PushDumpToQueue(string path)
        {
            _logger.Information("Reading dump file");
            var dump = File.ReadAllText(path);
            _logger.Information("Deserialize dump in memory");
            var states = _converter.Deserialize<QueueState[]>(dump);
            using (var model = _connection.CreateModel())
            {
                foreach (var state in states)
                {
                    _logger.Information($"Creating queue {state.Name}");
                    model.QueueDeclare(
                        queue: state.Name,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    _logger.Information($"Handling messages queue {state.Name}");
                    foreach (var message in state.Messages)
                    {
                        var props = model.CreateBasicProperties();
                        message.EnrichBasicProperties(props);
                        model.BasicPublish("", state.Name, props, message.Body);
                    }
                }
            }
            _logger.Information($"All queues handled");
        }
    }
}
