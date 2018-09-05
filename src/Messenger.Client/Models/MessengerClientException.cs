namespace Messenger.Client.Models
{
    using System;
    using System.Runtime.Serialization;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class MessengerClientException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public MessengerClientException()
        {
        }

        public MessengerClientException(string message) : base(message)
        {
        }

        public MessengerClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MessengerClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}