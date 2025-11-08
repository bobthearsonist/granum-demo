using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Infrastructure.Validation;

/// <summary>
/// Extension methods for validating entities using a two-phase validation approach.
/// </summary>
public static class EntityValidationExtensions
{
    /// <summary>
    /// Validates an entity using Data Annotations and FluentValidation (if registered).
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to validate. Must be a class.</typeparam>
    /// <param name="entity">The entity instance to validate.</param>
    /// <param name="services">The service provider used to resolve FluentValidation validators.</param>
    /// <returns>
    /// A tuple containing:
    /// - IsValid: true if validation passes, false otherwise
    /// - ErrorResult: An IActionResult with validation errors (BadRequestObjectResult for Data Annotations, 
    ///   UnprocessableEntityObjectResult for FluentValidation), or null if validation passes
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method implements a two-phase validation approach:
    /// </para>
    /// <para>
    /// 1. Data Annotations Validation (Phase 1):
    ///    - Validates attributes like [Required], [StringLength], [Range], etc.
    ///    - Returns 400 Bad Request if validation fails
    ///    - Use Data Annotations for simple, declarative validations (e.g., required fields, length constraints)
    /// </para>
    /// <para>
    /// 2. FluentValidation (Phase 2):
    ///    - Executed only if Data Annotations validation passes
    ///    - Validates complex business rules using IValidator&lt;TEntity&gt;
    ///    - Returns 422 Unprocessable Entity if validation fails
    ///    - Optional - gracefully handles when no validator is registered
    ///    - Use FluentValidation for complex business rules, cross-property validation, and conditional logic
    /// </para>
    /// <para>
    /// Return Status Codes:
    /// - 400 Bad Request: Data Annotations validation failed (malformed input)
    /// - 422 Unprocessable Entity: FluentValidation failed (business rule violation)
    /// - null: Validation passed
    /// </para>
    /// </remarks>
    /// <example>
    /// Usage in a controller:
    /// <code>
    /// var (isValid, errorResult) = await entity.ValidateAsync(HttpContext.RequestServices);
    /// if (!isValid) return errorResult!;
    /// </code>
    /// </example>
    public static async Task<(bool IsValid, IActionResult? ErrorResult)> ValidateAsync<TEntity>(
        this TEntity entity, IServiceProvider services) where TEntity : class
    {
        // Phase 1: Data Annotations validation
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(entity, new ValidationContext(entity), results, true))
            return (false, new BadRequestObjectResult(new ValidationProblemDetails(
                results.ToDictionary(e => e.MemberNames.First(), e => new[] { e.ErrorMessage! }))));

        // Phase 2: FluentValidation (optional)
        if (services.GetService(typeof(IValidator<TEntity>)) is not IValidator validator)
            return (true, null);

        var result = await validator.ValidateAsync(new ValidationContext<object>(entity));
        return result.IsValid
            ? (true, null)
            : (false, new UnprocessableEntityObjectResult(
                new ValidationProblemDetails(result.ToDictionary()) { Status = 422 }));
    }
}
