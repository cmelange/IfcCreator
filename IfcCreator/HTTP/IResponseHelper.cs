using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace IfcCreator.HTTP
{
    public interface IResponseHelper
    {
        Task PostCommand<T>(Func<T, Task> function,
                            T input,
                            HttpContext httpContext);
    }
}