using Newtonsoft.Json;
using Restaurant.Business.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Business.Responses
{
    [JsonObject]
    public class ResponseObject<T> : List<T>, IResponseObject<T>
    {
        public ResponseObject(List<T> obj, Exception ex)
        {
            Result = obj != null ? 1 : 0;
            Records = obj;
            Ex = ex;
        }
        public int Result { get; private set; }
        public IEnumerable<T> Records { get; private set; }
        public Exception Ex { get; private set; }
    }
}
