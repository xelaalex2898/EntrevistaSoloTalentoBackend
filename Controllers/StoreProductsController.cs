using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntrevistaSoloTalentoBackend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace EntrevistaSoloTalentoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreProductsController : ControllerBase
    {
        private readonly EntrevistaSoloTalentoContext _context;

        public StoreProductsController(EntrevistaSoloTalentoContext context)
        {
            _context = context;
        }

        // GET: api/StoreProducts
        [HttpGet]
        [Route("storeProducts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<StoreProducts>>> GetStoreProducts(int id)
        {
          if (_context.StoreProducts == null)
          {
              return NotFound(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
          }
            try
            {
                List<StoreProducts> storeProducts = await _context.StoreProducts.Where(sp=>sp.Store==id). ToListAsync();
                return Ok (new { error = false, message = storeProducts, statusCode = 200 }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message, statusCode = 401 });
            }
        }

        // POST: api/StoreProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StoreProducts>> PostStoreProducts(StoreProducts storeProducts)
            
        {
            if (_context.StoreProducts == null)
            {
                return NotFound(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
            }
            StoreProducts product= new StoreProducts();
            try
            {
                int id = _context.StoreProducts.OrderByDescending(x => x.StoreProductID).Select(x => x.StoreProductID).FirstOrDefault();
                product.StoreProductID = id+1;
                product.RegistrationDate = DateTime.Now;
                product.Store = storeProducts.Store;
                product.Product= storeProducts.Product;
                _context.StoreProducts.Add(product);
                _context.SaveChanges();
                return Ok(new { error = false, message = "Producto registrado en la tienda correctamente", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message, statusCode = 401 });
            }
        }

        // DELETE: api/StoreProducts/5
        [HttpDelete("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<IActionResult> DeleteStoreProducts(int id)
        {
            if (_context.StoreProducts == null)
            {
                return NotFound();
            }
            var storeProducts = await _context.StoreProducts.FindAsync(id);
            if (storeProducts == null)
            {
                return NotFound();
            }

            _context.StoreProducts.Remove(storeProducts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
