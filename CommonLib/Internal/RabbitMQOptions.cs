using System;

namespace CommonLib.Internal
{
    internal class RabbitMQOptions
    {
        public Uri? Host { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public bool HasMissingValues =>
               Host is not null
            && Username is not null
            && Password is not null;
    }
}
