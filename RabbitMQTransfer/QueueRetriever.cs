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

        public QueueRetriever(IConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public void DumpQueuesToFile(string[] queues, string outputFile)
        {
            using (var model = _connection.CreateModel())
            {
                var states = new List<QueueState>();
                foreach (var name in queues)
                {
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
                }

                DumpStateToFile(states, outputFile);
            }
        }

        private void DumpStateToFile(List<QueueState> states, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(JsonConvert.SerializeObject(states.ToArray()));
            }
        }
    }
}
