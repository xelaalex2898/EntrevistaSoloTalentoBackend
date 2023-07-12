using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntrevistaSoloTalentoBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Cors;

namespace EntrevistaSoloTalentoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly EntrevistaSoloTalentoContext _context;
        public SalesController(IConfiguration configuration, EntrevistaSoloTalentoContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // POST: api/Sales
        [Authorize]
        [HttpPost]
        public ActionResult sales(Sales sales)
        {
            if (_context.Sales == null)
            {

                return BadRequest(new { error = true, message = "ha ocurrido un error revisa el proceso", statusCode = 401 });
            }
            try
            {
                var productID = sales.Product;
                var clientID = sales.Client;
                //int id = _context.Sales.OrderByDescending(x => x.SaleID).Select(x => x.SaleID).FirstOrDefault();
                Sales sale = new Sales();
                //sale.SaleID = id + 1;
                sale.Client = clientID;
                sale.Date = DateTime.Now;
                sale.Bought = false;
                sale.Product = productID;
                _context.Sales.Add(sale);
                _context.SaveChanges();
                return Ok(new { error = false, message = "operación exitosa", statusCode = 200 });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = true, message = ex.Message, statusCode = 401 });
            }

        }
        [Authorize]
        [Route("cart")]
        [HttpGet]
        public ActionResult Cart(int id)
        {
            if (_context.Users == null)
            {
                return NotFound(new { error = true, message = "ha ocurrido un error revise el proceso", statusCode = 400});
            }
            try
            {
                List<Sales> cart = _context.Sales.Where(v => v.Bought == false && v.Client == id).ToList();

                if (cart == null)
                {
                    
                    return Ok(new { error = false, message = "no ha registrado articulos", statusCode = 200 });
                }
                
                return Ok(new { error = false, message = cart, statusCode = 200 });



            }
            catch (Exception ex)
            {
                return BadRequest(new { error = false, message = ex.Message, statusCode = 200 });
            }
        }

        // DELETE: api/Sales/5
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSales(int id)
        {
            if (_context.Sales == null)
            {
                return NotFound();
            }
            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sales);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
