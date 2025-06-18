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
    public class ComparacionesController : ControllerBase
    {
        private readonly ImagenDbContext _context;
        private readonly IImageProcessorService _processor;

        public ComparacionesController(
            ImagenDbContext context,
            IImageProcessorService processor
        )
        {
            _context = context;
            _processor = processor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComparacionResponseDto>>> GetTodas()
        {
            var lista = await _context.Comparaciones
                .AsNoTracking()
                .Select(c => new ComparacionResponseDto
                {
                    IdComparacion = c.IdComparacion,
                    IdImagenOriginal = c.IdImagenOriginal,
                    IdImagenProcesada = c.IdImagenProcesada,
                    Mse = (double)c.MSE,
                    Psnr = (double)c.PSNR,
                    ImagenDiferenciasBase64 = c.ImagenDiferencias == null ? null : Convert.ToBase64String(c.ImagenDiferencias),
                    FechaComparacion = c.FechaComparacion
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ComparacionResponseDto>> GetPorId(int id)
        {
            var entidad = await _context.Comparaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdComparacion == id);

            if (entidad == null)
                return NotFound(new { mensaje = "No se encontró ninguna comparación con ese ID." });

            var dto = new ComparacionResponseDto
            {
                IdComparacion = entidad.IdComparacion,
                IdImagenOriginal = entidad.IdImagenOriginal,
                IdImagenProcesada = entidad.IdImagenProcesada,
                Mse = (double)entidad.MSE,
                Psnr = (double)entidad.PSNR,
                ImagenDiferenciasBase64 = Convert.ToBase64String(entidad.ImagenDiferencias),
                FechaComparacion = entidad.FechaComparacion
            };

            return Ok(dto);
        }

        [HttpGet("porImagenes")]
        public async Task<ActionResult<ComparacionResponseDto>> GetPorIds(
            [FromQuery] int imgOriginalId,
            [FromQuery] int imgProcesadaId)
        {
            if (imgOriginalId <= 0 || imgProcesadaId <= 0)
                return BadRequest(new { mensaje = "IDs de imágenes inválidos." });

            var entidad = await _context.Comparaciones
                .AsNoTracking()
                .Where(x => x.IdImagenOriginal == imgOriginalId && x.IdImagenProcesada == imgProcesadaId)
                .Select(x => new ComparacionResponseDto
                {
                    IdComparacion = x.IdComparacion,
                    IdImagenOriginal = x.IdImagenOriginal,
                    IdImagenProcesada = x.IdImagenProcesada,
                    Mse = (double)x.MSE,
                    Psnr = (double)x.PSNR,
                    ImagenDiferenciasBase64 = Convert.ToBase64String(x.ImagenDiferencias),
                    FechaComparacion = x.FechaComparacion
                })
                .FirstOrDefaultAsync();

            if (entidad == null)
                return NotFound(new { mensaje = "No se encontró una comparación para esos IDs de imágenes." });

            return Ok(entidad);
        }

        [HttpPost("comparar")]
        public async Task<ActionResult<ComparacionResponseDto>> Comparar([FromBody] CompararDto dto)
        {
            if (dto == null || dto.IdImagenOriginal <= 0 || dto.IdImagenProcesada <= 0)
                return BadRequest(new { mensaje = "Los IDs de imagen enviados son inválidos." });

            var orig = await _context.Imagenes.FindAsync(dto.IdImagenOriginal);
            var proc = await _context.ImagenesProcesadas.FindAsync(dto.IdImagenProcesada);

            if (orig == null || proc == null)
                return NotFound(new { mensaje = "La imagen original o la procesada no existen." });

            var (mse, psnr, diffBytes) =
                await _processor.CompararAsync(orig.DatosImagen, proc.DatosProcesados);

            var entidad = new Comparacion
            {
                IdImagenOriginal = dto.IdImagenOriginal,
                IdImagenProcesada = dto.IdImagenProcesada,
                MSE = (float)mse,
                PSNR = (float)psnr,
                ImagenDiferencias = diffBytes,
                FechaComparacion = DateTime.UtcNow
            };

            _context.Comparaciones.Add(entidad);
            await _context.SaveChangesAsync();

            var resp = new ComparacionResponseDto
            {
                IdComparacion = entidad.IdComparacion,
                IdImagenOriginal = entidad.IdImagenOriginal,
                IdImagenProcesada = entidad.IdImagenProcesada,
                Mse = mse,
                Psnr = psnr,
                ImagenDiferenciasBase64 = Convert.ToBase64String(diffBytes),
                FechaComparacion = entidad.FechaComparacion
            };

            return CreatedAtAction(nameof(GetPorId), new { id = entidad.IdComparacion }, resp);
        }
    }
}
