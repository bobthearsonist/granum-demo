using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Infrastructure;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateModelAttribute() : TypeFilterAttribute(typeof(ValidateModelFilter));
