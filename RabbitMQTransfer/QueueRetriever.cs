using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace RabbitMQTransfer
{
    public class QueueRetriever
    {
        private IConnection _connection;
        private ILogger _logger;
        private JsonConverter _converter;

        public QueueRetriever(IConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
            _converter = new JsonConverter();
        }

        public void DumpQueuesToFile(string[] queues, string outputFile)
        {
            _logger.Information("Start reading queues");
            using (var model = _connection.CreateModel())
            {
                var states = new List<QueueState>();
                foreach (var name in queues)
                {
                    _logger.Information($"Start reading queue {name}");
                    var state = new QueueState()
                    {
                        Name = name
                    };
                    states.Add(state);
                    
                    var nextMessage = model.BasicGet(name, false);
                    while (nextMessage != null)
                    {
                        state.Messages.Add(new Message(nextMessage.BasicProperties, nextMessage.Body));
                        nextMessage = model.BasicGet(name, false);
                    }
                    _logger.Information($"All messages received for queue {name}");
                }

                _logger.Information("Dumping state to file");
                DumpStateToFile(states, outputFile);
            }
            _logger.Information("State dumped");
        }

        private void DumpStateToFile(List<QueueState> states, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(_converter.Serialize(states.ToArray()));
            }
        }
    }
}
