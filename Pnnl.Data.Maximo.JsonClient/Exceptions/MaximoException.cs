using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;

namespace Pnnl.Data.Maximo.JsonClient.Exceptions
{
    public class MaximoException
    {
        public Error Error { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }

    public class Error
    {
        public Extendederror extendedError { get; set; }
        public string reasonCode { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }

    public class Extendederror
    {
        public Moreinfo moreInfo { get; set; }
    }

    public class Moreinfo
    {
        public string href { get; set; }
    }
}
