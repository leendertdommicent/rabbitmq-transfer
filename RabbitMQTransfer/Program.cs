using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace RabbitMQTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger log = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();
            try
            {
                Start(log);
            }
            catch (Exception e)
            {
                log.Error(e,"Exception occured");
            }
            
            Console.In.ReadLine();
        }

        static void Start(ILogger log)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            log.Debug("Creating connection");
            using (var connection = new MqConnectionFactory(log, configuration).Create())
            {
                log.Debug("Check for input queues");
                var inputqueues = configuration.GetSection("InputQueues").GetChildren().Select(x => x.Value).ToArray();

                if (inputqueues.Length != 0)
                {
                    log.Debug("Input queues found, starting in retrieval mode");
                    var outputFile = configuration["OutputFile"];
                    if (String.IsNullOrEmpty(outputFile))
                    {
                        log.Error("No output file found!");
                        return;
                    }
                    new QueueRetriever(connection, log).DumpQueuesToFile(inputqueues, outputFile);

                }
                else
                {
                    log.Debug("No input queues found, searching for input file");
                    var inputFile = configuration["InputFile"];
                    if (String.IsNullOrEmpty(inputFile))
                    {
                        log.Error("No input file found!");
                        return;
                    }
                    new QueueSender(connection, log).PushDumpToQueue(inputFile);
                }
            }

            
        }
    }
}
