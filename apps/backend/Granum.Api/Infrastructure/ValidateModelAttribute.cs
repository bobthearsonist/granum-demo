using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Granum.Api.Infrastructure;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        foreach (var arg in ctx.ActionArguments.Values.Where(v => v != null))
        {
            var vType = typeof(IValidator<>).MakeGenericType(arg!.GetType());
            var validator = ctx.HttpContext.RequestServices.GetService(vType) as IValidator;
            if (validator is null) continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(arg));
            if (result.IsValid) continue;

            ctx.Result = new UnprocessableEntityObjectResult(
                new ValidationProblemDetails(result.ToDictionary()) { Status = 422 }
            );
            return;
        }

        await next();
    }
}