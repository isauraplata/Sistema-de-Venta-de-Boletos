using Microsoft.AspNetCore.Mvc;
using TicketingSystem.DTOs.Ticket;
using TicketingSystem.Interfaces;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketResponseDTO>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{uuid:guid}")]
        public async Task<ActionResult<TicketResponseDTO>> GetTicket(Guid uuid)
        {
            var ticket = await _ticketService.GetTicketByUuidAsync(uuid);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDTO>> CreateTicket([FromBody] TicketCreateDTO ticketDto)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(ticketDto);
            return CreatedAtAction(
                nameof(GetTicket),
                new { uuid = createdTicket.Uuid },
                createdTicket);
        }

        [HttpPut("{uuid:guid}")]
        public async Task<ActionResult> UpdateTicket(Guid uuid, [FromBody] TicketCreateDTO ticketDto)
        {
            var success = await _ticketService.UpdateTicketAsync(uuid, ticketDto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{uuid:guid}")]
        public async Task<ActionResult> DeleteTicket(Guid uuid)
        {
            var success = await _ticketService.DeleteTicketAsync(uuid);
            if (!success) return NotFound();
            return NoContent();
        }
    }

}
