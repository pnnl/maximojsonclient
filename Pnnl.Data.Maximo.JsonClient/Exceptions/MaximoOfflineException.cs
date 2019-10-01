using System;

namespace Pnnl.Data.Maximo.JsonClient.Exceptions
{
    public class MaximoOfflineException : Exception
    {
        public MaximoOfflineException() { }

        public MaximoOfflineException(string message) : base(message) { }
    }
}
