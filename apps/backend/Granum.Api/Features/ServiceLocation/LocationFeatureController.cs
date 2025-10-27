using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.ServiceLocation;

[ApiController]
[Route("api/service-locations/{locationId}/features")]
public class LocationFeatureController(ILocationFeatureService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int locationId) =>
        Ok(await service.GetByLocationIdAsync(locationId));

    [HttpGet("{featureId}")]
    public async Task<IActionResult> Get(int locationId, int featureId) =>
        Ok(await service.GetByIdAsync(featureId));

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create(int locationId, [FromBody] LocationFeature feature)
    {
        feature.LocationId = locationId;
        var createdFeature = await service.CreateAsync(feature);
        return CreatedAtAction(nameof(Get), new { locationId, featureId = createdFeature.Id }, createdFeature);
    }

    [HttpPatch("{featureId}")]
    public async Task<IActionResult> Update(int locationId, int featureId, [FromBody] JsonPatchDocument<LocationFeature> patchDoc)
    {
        var feature = await service.GetByIdAsync(featureId);

        patchDoc.ApplyTo(feature, ModelState);
        if (!ModelState.IsValid)
            return BadRequest(patchDoc);

        await service.UpdateAsync(feature);
        return NoContent();
    }

    [HttpDelete("{featureId}")]
    public async Task<IActionResult> Delete(int locationId, int featureId)
    {
        await service.DeleteAsync(featureId);
        return NoContent();
    }
}
