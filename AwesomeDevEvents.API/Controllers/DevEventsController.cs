using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
  [Route("api/dev-events")]
  [ApiController]

  public class DevEventsController : ControllerBase
  {
    private readonly DevEventsDbContext _context;

    public DevEventsController(DevEventsDbContext context)
    {
      _context = context;
    }

    // api/dev-events GET
    [HttpGet]
    public IActionResult GetAll()
    {
      var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();
      return Ok(devEvents); //Ok = Objeto do código de resposta 200
    }

    // api/dev-events/id GET
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
      var devEvent = _context.DevEvents.Include(de => de.Speakers).SingleOrDefault(d => d.Id == id);
      if (devEvent == null)
      {
        return NotFound(); //Not Found = Objeto do código de resposta 404
      }
      return Ok(devEvent);
    }

    // api/dev-events POST
    [HttpPost]
    public IActionResult Post(DevEvent devEvent)
    {
      _context.DevEvents.Add(devEvent);
      _context.SaveChanges();

      return CreatedAtAction(nameof(GetById), new { id = devEvent.Id}, devEvent);
      //CreateAtAction = Objeto do código de resposta 201 junto com o objeto criado
    }

    // api/dev-events/id PUT
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, DevEvent input)
    {
      var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
      if (devEvent == null)
      {
        return NotFound();
      }

      devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

      _context.DevEvents.Update(devEvent);
      _context.SaveChanges();
      return NoContent(); //Not Found = Objeto do código de resposta 204
    }

    // api/dev-events/id DELETE
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
      var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
      if (devEvent == null)
      {
        return NotFound();
      }

      devEvent.Delete();
      _context.SaveChanges();
      return NoContent();
    }

    // api/dev-events/id/speakers
    [HttpPost("{id}/speakers")]
    public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
    {
      speaker.DevEventId = id;

      var devEvent = _context.DevEvents.Any(d => d.Id == id);
      if (!devEvent)
      {
        return NotFound();
      }

      _context.DevEventSpeakers.Add(speaker);
      _context.SaveChanges();
      return NoContent();
    }
  }
}