using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Mvc;

namespace IfcCreator.ExceptionHandling
{
    /// <summary>
    /// This class serves as the base class for all Exceptions that can be reported via HTTP according to RFC7808
    /// For that purpose it extends the System.Exception with a member of type ProblemDetails
    /// <summary>
    public class ReportableException : Exception
    {
        public int status { get { return details.Status ?? 500; } }
        private ProblemDetails details;

        public ReportableException() {
            CreateProblemDetails();
        }

        public ReportableException(string message): base(message) {
            CreateProblemDetails(message);
        }

        public ReportableException(string message, Exception inner): base(message, inner) {
            CreateProblemDetails(message);
        }

        protected ReportableException(ProblemDetails details): base(details.Detail)
        {
            this.details = details;
        }

        protected ReportableException(ProblemDetails details, Exception inner): base(details.Detail, inner)
        {
            this.details = details;
        }

        private void CreateProblemDetails(string detail = "Oops. An unexpected server error occured")
        {
            this.details = new ProblemDetails();
            this.details.Title = "Internal Server Error";
            this.details.Status = 500;
            this.details.Detail = detail;
        }

        public virtual string ToJSON()
        {
            return JsonConvert.SerializeObject(this.details);
        }
    }
}