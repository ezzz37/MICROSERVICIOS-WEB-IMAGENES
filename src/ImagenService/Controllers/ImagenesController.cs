using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImagenService.Data;
using ImagenService.DTOs;
using ImagenService.Models;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace ImagenService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly ImagenDbContext _context;
        public ImagenesController(ImagenDbContext db) => _context = db;

        // GET: api/Imagenes
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<ImagenDto>>> GetTodas()
        {
            var lista = await _context.Imagenes.ToListAsync();

            var resultado = lista.Select(i => new ImagenDto
            {
                IdImagen = i.IdImagen,
                Nombre = i.Nombre,
                DatosImagenBase64 = Convert.ToBase64String(i.DatosImagen)
            });

            return Ok(resultado);
        }

        // GET: api/Imagenes/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<Imagen>> GetPorId(int id)
        {
            var img = await _context.Imagenes
                               .Include(i => i.ImagenesProcesadas)
                               .Include(i => i.ComparacionesOriginal)
                               .FirstOrDefaultAsync(i => i.IdImagen == id);
            if (img == null) return NotFound();
            return Ok(img);
        }

        // POST: api/Imagenes
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Imagen>> Crear([FromBody] Imagen modelo)
        {
            modelo.FechaCarga = DateTime.UtcNow;
            _context.Imagenes.Add(modelo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPorId), new { id = modelo.IdImagen }, modelo);
        }

        // POST: api/Imagenes/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<ActionResult<Imagen>> Upload([FromForm] ImagenUploadDto dto)
        {
            byte[] datos;
            using (var ms = new MemoryStream())
            {
                await dto.Archivo.CopyToAsync(ms);
                datos = ms.ToArray();
            }

            int ancho = 0, alto = 0;
            try
            {
                using var img = Image.FromStream(new MemoryStream(datos));
                ancho = img.Width;
                alto = img.Height;
            }
            catch
            {
                // Si falla, deja 0
            }

            var entidad = new Imagen
            {
                Nombre = dto.Nombre,
                DatosImagen = datos,
                AnchoOriginal = ancho,
                AltoOriginal = alto,
                FechaCarga = DateTime.UtcNow
            };

            _context.Imagenes.Add(entidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = entidad.IdImagen }, entidad);
        }

        // PUT: api/Imagenes/5
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Imagen modelo)
        {
            if (id != modelo.IdImagen) return BadRequest();
            _context.Entry(modelo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Imagenes.Any(e => e.IdImagen == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Imagenes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var img = await _context.Imagenes.FindAsync(id);
            if (img == null) return NotFound();

            _context.Imagenes.Remove(img);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}