using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Granum.Api.Infrastructure;

public class ValidateModelFilter(ILogger<ValidateModelFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        logger.LogInformation("ValidateModelFilter executing for action: {Action}", ctx.ActionDescriptor.DisplayName);
        
        foreach (var arg in ctx.ActionArguments.Values.Where(v => v != null))
        {
            var vType = typeof(IValidator<>).MakeGenericType(arg!.GetType());
            var validator = ctx.HttpContext.RequestServices.GetService(vType) as IValidator;
            
            logger.LogInformation("Checking validator for type {Type}: {ValidatorFound}", arg!.GetType().Name, validator != null);
            
            if (validator is null) continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(arg));
            logger.LogInformation("Validation result for {Type}: Valid={IsValid}, Errors={ErrorCount}", arg.GetType().Name, result.IsValid, result.Errors.Count);
            
            if (result.IsValid) continue;

            ctx.Result = new UnprocessableEntityObjectResult(
                new ValidationProblemDetails(result.ToDictionary()) { Status = 422 }
            );
            return;
        }

        await next();
    }
}
