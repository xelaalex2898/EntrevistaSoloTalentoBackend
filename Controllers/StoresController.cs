using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntrevistaSoloTalentoBackend.Models;
using Microsoft.AspNetCore.Cors;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace EntrevistaSoloTalentoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly EntrevistaSoloTalentoContext _context;

        public StoresController(EntrevistaSoloTalentoContext context)
        {
            _context = context;
        }

        // GET: api/Stores
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Store>>> GetStore()
        {
          if (_context.Store == null)
          {
              return NotFound();
          }
            return await _context.Store.ToListAsync();
        }

        // POST: api/Stores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Store>> PostStore(Store info)
        {

          if (_context.Store == null)
          {
              return Problem("Entity set 'EntrevistaSoloTalentoContext.Store'  is null.");
          }
            Store store = new Store();
            try
            {
                int id = _context.Store.OrderByDescending(x => x.IDStore).Select(x => x.IDStore).FirstOrDefault();
                store.Direction = info.Direction;
                store.Branch = info.Branch;
                store.IDStore = id+1;
                _context.Store.Add(store);
                await _context.SaveChangesAsync();
                return Ok(new { error = false, statusCode = 200, message = "registro exitoso" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // DELETE: api/Stores/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            if (_context.Store == null)
            {
                return NotFound();
            }
            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Store.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
