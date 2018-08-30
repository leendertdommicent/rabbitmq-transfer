using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

namespace RabbitMQTransfer
{
    public class MqConnectionFactory
    {
        private ILogger _logger;
        private IConfiguration _configuration;

        public MqConnectionFactory(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IConnection Create()
        {
            var rabbitMqSection = _configuration.GetSection("RabbitMQ");
            var host = RetrieveParameter(rabbitMqSection, "Hostname");
            var port = RetrieveParameter(rabbitMqSection, "Port");
            var username = RetrieveParameter(rabbitMqSection, "Username");
            var password = RetrieveParameter(rabbitMqSection, "Password");

            return new ConnectionFactory()
            {
                HostName = host,
                Port = Int32.Parse(port),
                UserName = username,
                Password = password
            }.CreateConnection();
        }

        private string RetrieveParameter(IConfiguration section, string key)
        {
            var value = section[key];
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"No configuration for ${key} found.");
            }

            return value;
        }
    }
}
