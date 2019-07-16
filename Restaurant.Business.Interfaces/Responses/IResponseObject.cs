using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Business.Interfaces.Responses
{
    public interface IResponseObject<T>
    {
        int Result { get; }
        IEnumerable<T> Records { get; }
        Exception Ex { get; }
    }
}
