using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Filter
{
    public class APILoggingFilter : IActionFilter
    {
        private readonly ILogger<APILoggingFilter> _logger;
        public APILoggingFilter(ILogger<APILoggingFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuting");
            _logger.LogInformation("##############################################");
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()}");
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
            _logger.LogInformation("##############################################");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("##############################################");
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()}");
            _logger.LogInformation("##############################################");
        }
    }
}
