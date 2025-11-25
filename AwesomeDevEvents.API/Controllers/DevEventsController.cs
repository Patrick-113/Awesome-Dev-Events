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

    /// <summary>
    /// Obter todos os eventos
    /// </summary>
    /// <returns>Coleção de Eventos</returns>
    /// <response code="200">Sucesso</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
      var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();
      return Ok(devEvents); //Ok = Objeto do código de resposta 200
    }

    /// <summary>
    /// Obter um evento
    /// </summary>
    /// <param name="id">Identificar do evento</param>
    /// <returns>Dados do Evento</returns>
    /// <response code="200">Sucesso</response>
    /// <response code="404">Não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
      var devEvent = _context.DevEvents.Include(de => de.Speakers).SingleOrDefault(d => d.Id == id);
      if (devEvent == null)
      {
        return NotFound(); //Not Found = Objeto do código de resposta 404
      }
      return Ok(devEvent);
    }

    /// <summary>
    /// Cadastrar um evento
    /// </summary>
    /// <remarks>
    /// {"title": "string", "description": "string", "startDate": "timestamp", "endDate": "timestamp"}
    /// </remarks>
    /// <param name="devEvent">Dados do evento</param>
    /// <returns>Objeto recém-criado</returns>
    /// <response code="201">Sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Post(DevEvent devEvent)
    {
      _context.DevEvents.Add(devEvent);
      _context.SaveChanges();

      return CreatedAtAction(nameof(GetById), new { id = devEvent.Id}, devEvent);
      //CreateAtAction = Objeto do código de resposta 201 junto com o objeto criado
    }

    /// <summary>
    /// Atualizar um evento
    /// </summary>
    /// <remarks>
    /// {"title": "string", "description": "string", "startDate": "timestamp", "endDate": "timestamp"}
    /// </remarks>
    /// <param name="id">Edentificador do evento</param>
    /// <param name="input">Dados do Evento</param>
    /// <returns>Nada.</returns>
    /// <response code="204">Sucesso</response>
    /// <response code="404">Não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletar um evento
    /// </summary>
    /// <param name="id">Identificador do evento</param>
    /// <returns>Nada.</returns>
    /// <response code="204">Sucesso</response>
    /// <response code="404">Não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Cadastrar palestrante
    /// </summary>
    /// <remarks>
    /// {"name": "string", "talkTitle": "string", "talkDescription": "string", "linkedInProfile": "string", "devEventId": "Guid"}
    /// </remarks>
    /// <param name="id">Identificador do evento</param>
    /// <param name="speaker">Dados do palestrante</param>
    /// <returns>Nada</returns>
    /// <response code="204">Sucesso</response>
    /// <response code="404">Evento não encontrado</response>
    [HttpPost("{id}/speakers")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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