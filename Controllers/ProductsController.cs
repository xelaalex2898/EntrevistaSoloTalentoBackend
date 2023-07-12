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
    public class ProductsController : ControllerBase
    {
        private readonly EntrevistaSoloTalentoContext _context;

        public ProductsController(EntrevistaSoloTalentoContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.ToListAsync();
        }
        [HttpGet]
        [Route("product")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Products>>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound(new { error = true, message = "Ha ocurrido un error, por favor revisa el proceso", statusCode = 401 });
            }
            try
            {
                List<Products> Product = await _context.Products.Where(p => p.Code == id).ToListAsync();
                return Ok(new { error = false, message = Product, statusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message, statusCode = 401 });
            }
        }


        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Products>> PostProducts(Products products)
        {
          if (_context.Products == null)
          {
              return BadRequest(new { error = true, message = "No hay productos registrados", statusCode = 401 });
          }
            Products product = new Products();
            try
            {
                int id = _context.Products.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
                product.Description = products.Description;
                product.Price=products.Price ;
                product.Image=products.Image ;
                product.Stock=products.Stock ;
                product.Code = id + 1;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Ok(new { error = false, message = string.Format("El producto con código {0} registrado correctamente", product.Code), statusCode = 200 });
            }
            catch (Exception ex)
            {
                return Conflict(new { error = true, message = ex.Message, statusCode = 200 });
            }
        }

        // DELETE: api/Products/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
