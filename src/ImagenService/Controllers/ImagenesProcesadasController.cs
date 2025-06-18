using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImagenService.Data;
using ImagenService.DTOs;
using ImagenService.Models;
using ImagenService.Services;


namespace ImagenService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesProcesadasController : ControllerBase
    {
        private readonly ImagenDbContext _context;
        private readonly IImageProcessorService _processor;

        public ImagenesProcesadasController(ImagenDbContext db, IImageProcessorService processor)
        {
            _context = db;
            _processor = processor;
        }

        // GET: api/ImagenesProcesadas
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<ImagenProcesadaSimpleDto>>> GetTodas()
        {
            var lista = await _context.ImagenesProcesadas
                .AsNoTracking()
                .Select(ip => new ImagenProcesadaSimpleDto
                {
                    IdImagenProcesada = ip.IdImagenProcesada,
                    IdImagenOriginal = ip.IdImagenOriginal
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/ImagenesProcesadas/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ImagenProcesadaResponseDto>> GetPorId(int id)
        {
            var item = await _context.ImagenesProcesadas
                .Include(ip => ip.ImagenOriginal)
                .Include(ip => ip.AlgoritmoCompresion)
                .Include(ip => ip.Comparaciones)
                .FirstOrDefaultAsync(ip => ip.IdImagenProcesada == id);

            if (item == null)
                return NotFound();

            var dto = new ImagenProcesadaResponseDto
            {
                IdImagenProcesada = item.IdImagenProcesada,
                AnchoResolucion = item.AnchoResolucion,
                AltoResolucion = item.AltoResolucion,
                ProfundidadBits = item.ProfundidadBits,
                FechaProcesamiento = item.FechaProcesamiento,
                IdAlgoritmoCompresion = item.IdAlgoritmoCompresion.GetValueOrDefault(),
                IdImagenOriginal = item.IdImagenOriginal,
                RatioCompresion = item.RatioCompresion,
                ImagenOriginal = item.ImagenOriginal?.Nombre ?? string.Empty,
                Algoritmo = item.AlgoritmoCompresion?.NombreAlgoritmo ?? string.Empty,
                DatosProcesadosBase64 = item.DatosProcesados != null
                                        ? Convert.ToBase64String(item.DatosProcesados)
                                        : string.Empty
            };

            return Ok(dto);
        }

        // POST: api/ImagenesProcesadas/procesar/{idImagen}
        [HttpPost("procesar/{idImagen}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<ImagenProcesadaResponseDto>> Procesar(int idImagen, [FromBody] ProcesarDto dto)
        {
            var orig = await _context.Imagenes.FindAsync(idImagen);
            if (orig == null)
                return NotFound($"No existe imagen con Id {idImagen}");

            if (dto.AnchoResolucion <= 0 || dto.AltoResolucion <= 0)
                return BadRequest("AnchoResolucion y AltoResolucion deben ser mayores que cero.");

            if (dto.ProfundidadBits is not (1 or 8 or 24))
                return BadRequest("ProfundidadBits debe ser 1, 8 o 24.");

            var formato = dto.Algoritmo?.ToUpperInvariant();
            if (formato != "JPEG" && formato != "PNG")
                return BadRequest("Algoritmo desconocido. Use 'JPEG' o 'PNG'.");

            var existeAlgo = await _context.AlgoritmosCompresion
                .AnyAsync(a => a.IdAlgoritmoCompresion == dto.IdAlgoritmoCompresion);
            if (!existeAlgo)
                return BadRequest($"No existe AlgoritmoCompresion con Id {dto.IdAlgoritmoCompresion}.");

            // 1) Muestreo
            var muestreado = _processor.Muestrear(orig.DatosImagen, dto.AnchoResolucion, dto.AltoResolucion);

            // 2) Cuantización
            var cuantizado = _processor.Cuantizar(muestreado, dto.ProfundidadBits);

            // 3) Compresión
            var comprimido = _processor.Comprimir(cuantizado, formato!, dto.NivelCompresion);

            if (comprimido == null || comprimido.Length == 0)
                return StatusCode(500, "Error: el procesamiento de la imagen no generó datos.");

            // Calcular ratio de compresión
            var originalSize = orig.DatosImagen?.Length ?? 1;
            var compressedSize = comprimido?.Length ?? 1;
            double? ratioCompresion = null;
            if (compressedSize > 0)
                ratioCompresion = (double)originalSize / compressedSize;

            var procEnt = new ImagenProcesada
            {
                IdImagenOriginal = idImagen,
                AnchoResolucion = dto.AnchoResolucion,
                AltoResolucion = dto.AltoResolucion,
                ProfundidadBits = dto.ProfundidadBits,
                IdAlgoritmoCompresion = dto.IdAlgoritmoCompresion,
                DatosProcesados = comprimido,
                FechaProcesamiento = DateTime.UtcNow,
                RatioCompresion = ratioCompresion
            };

            _context.ImagenesProcesadas.Add(procEnt);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo guardar la imagen procesada. Verifica los datos enviados.");
            }

            var cargado = await _context.ImagenesProcesadas
                .Include(i => i.ImagenOriginal)
                .Include(i => i.AlgoritmoCompresion)
                .FirstOrDefaultAsync(i => i.IdImagenProcesada == procEnt.IdImagenProcesada);

            var dtoResp = new ImagenProcesadaResponseDto
            {
                IdImagenProcesada = cargado.IdImagenProcesada,
                AnchoResolucion = cargado.AnchoResolucion,
                AltoResolucion = cargado.AltoResolucion,
                ProfundidadBits = cargado.ProfundidadBits,
                FechaProcesamiento = cargado.FechaProcesamiento,
                IdAlgoritmoCompresion = cargado.IdAlgoritmoCompresion.GetValueOrDefault(),
                RatioCompresion = cargado.RatioCompresion,
                ImagenOriginal = cargado.ImagenOriginal?.Nombre ?? string.Empty,
                Algoritmo = cargado.AlgoritmoCompresion?.NombreAlgoritmo ?? string.Empty,
                DatosProcesadosBase64 = cargado.DatosProcesados != null
                                        ? Convert.ToBase64String(cargado.DatosProcesados)
                                        : string.Empty
            };

            return CreatedAtAction(nameof(GetPorId), new { id = dtoResp.IdImagenProcesada }, dtoResp);
        }

        // PUT: api/ImagenesProcesadas/5
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ImagenProcesada modelo)
        {
            if (id != modelo.IdImagenProcesada)
                return BadRequest();

            _context.Entry(modelo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ImagenesProcesadas.AnyAsync(e => e.IdImagenProcesada == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ImagenesProcesadas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var item = await _context.ImagenesProcesadas.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.ImagenesProcesadas.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}