using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntrevistaSoloTalentoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace EntrevistaSoloTalentoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly EntrevistaSoloTalentoContext _context;

        public ClientsController(EntrevistaSoloTalentoContext context)
        {
            _context = context;
        }
        
        [Authorize]
        [HttpGet]
        public dynamic GetClient(int id) 
        {
            if (_context.Clients == null)
            {
                return NotFound(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
            }
            Clients client = _context.Clients.Where(c=> c.UserID == id).FirstOrDefault();
            if (client == null)
            {
                return NotFound(new { error = true, message = "por favor complete su proceso de registro", statusCode = 401 });
            }
            return Ok(new { error = false, message = client, statusCode = 200 });
        }
        [HttpPost]
        [Authorize]
        [Route("clientRegister")]
        public dynamic clientRegister(Clients Client)
        {
            if (_context.Clients== null)
            {
                return BadRequest(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
            }


            Clients client = new Clients();

            try
            {

                int id = _context.Clients.OrderByDescending(x => x.IDClient).Select(x => x.IDClient).FirstOrDefault();
                
                client.IDClient= id+1;
                client.Direction=Client.Direction;
                client.Name=Client.Name;
                client.LastName=Client.LastName;
                client.UserID=Client.UserID;
                _context.Clients.Add(client);
                _context.SaveChanges();

                return Ok(new { error = false, message = string.Format("Cliente {0} registrado correctamente", client.Name), statusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message, statusCode = 500 });
            }
        }

    }
}
