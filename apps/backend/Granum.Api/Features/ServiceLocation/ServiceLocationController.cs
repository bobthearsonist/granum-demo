using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.ServiceLocation;

[ApiController]
[Route("api/service-locations")]
public class ServiceLocationController(IServiceLocationService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await service.GetByIdAsync(id));

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create([FromBody] ServiceLocation location)
    {
        var createdLocation = await service.CreateAsync(location);
        return CreatedAtAction(nameof(Get), new { id = createdLocation.Id }, createdLocation);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JsonPatchDocument<ServiceLocation> patchDoc)
    {
        var location = await service.GetByIdAsync(id);

        patchDoc.ApplyTo(location, ModelState);
        if (!ModelState.IsValid)
            return BadRequest(patchDoc);

        await service.UpdateAsync(location);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
