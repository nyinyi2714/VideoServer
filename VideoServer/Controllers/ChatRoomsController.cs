using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoModel;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        private readonly VideoSourceContext _context;

        public ChatRoomsController(VideoSourceContext context)
        {
            _context = context;
        }

        // GET: api/ChatRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRooms()
        {
            return await _context.ChatRooms.ToListAsync();
        }

        // GET: api/ChatRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoom>> GetChatRoom(int id)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(id);

            if (chatRoom == null)
            {
                return NotFound();
            }

            return chatRoom;
        }

        // PUT: api/ChatRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatRoom(int id, ChatRoom chatRoom)
        {
            if (id != chatRoom.ChatRoomId)
            {
                return BadRequest();
            }

            _context.Entry(chatRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatRoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChatRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChatRoom>> PostChatRoom(ChatRoom chatRoom)
        {
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoom", new { id = chatRoom.ChatRoomId }, chatRoom);
        }

        // DELETE: api/ChatRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatRoom(int id)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            _context.ChatRooms.Remove(chatRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatRoomExists(int id)
        {
            return _context.ChatRooms.Any(e => e.ChatRoomId == id);
        }
    }
}
