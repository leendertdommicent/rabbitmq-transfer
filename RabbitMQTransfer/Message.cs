using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQTransfer
{
    public class Message
    {
        public byte[] Body { get; set; }

        /// <summary>Application Id.</summary>
        public string AppId { get; set; }

        /// <summary>MIME content encoding.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>MIME content type.</summary>
        public string ContentType { get; set; }

        /// <summary>Application correlation identifier.</summary>
        public string CorrelationId { get; set; }

        /// <summary>Non-persistent (1) or persistent (2).</summary>
        public byte DeliveryMode { get; set; }

        /// <summary>Message expiration specification.</summary>
        public string Expiration { get; set; }

        /// <summary>
        /// Message header field table. Is of type <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public IDictionary<string, object> Headers { get; set; }

        /// <summary>Application message Id.</summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Sets <see cref="P:RabbitMQ.Client.IBasicProperties.DeliveryMode" /> to either persistent (2) or non-persistent (1).
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>Message priority, 0 to 9.</summary>
        public byte Priority { get; set; }

        /// <summary>Destination to reply to.</summary>
        public string ReplyTo { get; set; }

        /// <summary>Message timestamp.</summary>
        public long Timestamp { get; set; }

        /// <summary>Message type name.</summary>
        public string Type { get; set; }

        /// <summary>User Id.</summary>
        public string UserId { get; set; }

        public Message() { }

        public Message(IBasicProperties properties, byte[] body)
        {
            AppId = properties.AppId;
            ContentEncoding = properties.ContentEncoding;
            ContentType = properties.ContentType;
            CorrelationId = properties.CorrelationId;
            DeliveryMode = properties.DeliveryMode;
            Expiration = properties.Expiration;
            Headers = properties.Headers;
            MessageId = properties.MessageId;
            Persistent = properties.Persistent;
            Priority = properties.Priority;
            ReplyTo = properties.ReplyTo;
            Timestamp = properties.Timestamp.UnixTime;
            Type = properties.Type;
            UserId = properties.UserId;
            Body = body;
        }

        public void EnrichBasicProperties(IBasicProperties props)
        {
            props.AppId = AppId;
            props.ContentEncoding = ContentEncoding;
            props.ContentType = ContentType;
            props.CorrelationId = CorrelationId;
            props.DeliveryMode = DeliveryMode;
            props.Expiration = Expiration;
            props.Headers = Headers;
            props.MessageId = MessageId;
            props.Persistent = Persistent;
            props.Priority = Priority;
            props.ReplyTo = ReplyTo;
            props.Timestamp = new AmqpTimestamp(Timestamp);
            props.Type = Type;
            props.UserId = UserId;
        }
    }
}
