using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService userService)
        {
            _chatService = userService;
        }

        [HttpGet]
        public ActionResult<List<Message>> Get() => _chatService.Get();

        [HttpGet("{id:length(24)}", Name = "GetMessage")]
        public ActionResult<Message> Get(string id)
        {
            var message = _chatService.Get(id);

            if (message == null) return NotFound();

            return message;
        }

        [HttpPost]
        public ActionResult<Message> Create(Message message)
        {
            _chatService.Create(message);

            return CreatedAtRoute("GetMessage", new { id = message.Id.ToString() }, message);
        }


        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Message messageIn)
        {
            var message = _chatService.Get(id);

            if (message == null) return NotFound();

            _chatService.Update(id, messageIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var message = _chatService.Get(id);

            if (message == null) return NotFound();

            _chatService.Remove(message.Id);

            return NoContent();
        }
    }
}
