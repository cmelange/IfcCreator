using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IfcCreator.ExceptionHandling
{
    public class ValidationErrorActionResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new ValidationException(context.ModelState, "An error occurred while valitdating your input");
        }
    }
}