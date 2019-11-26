using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace IfcCreator.ExceptionHandling
{
    public class ValidationException: ReportableException
    {
        public ValidationException(IDictionary<string, string[]> details, string msg)
            : base(UpdateProblemDetails(new ValidationProblemDetails(details), msg))
        {}

        public ValidationException(ModelStateDictionary modelState, string msg)
            : base(UpdateProblemDetails(new ValidationProblemDetails(modelState), msg))
        {}

        private static ValidationProblemDetails UpdateProblemDetails(ValidationProblemDetails problemDetails,
                                                                     string msg)
        {        
            problemDetails.Title = "Bad Request";
            problemDetails.Detail = msg;
            problemDetails.Status = 400;
            return problemDetails;
        }
    }
}