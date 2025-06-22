using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ImagenService.DTOs
{
    public class ImagenUploadDto
    {
        [FromForm(Name = "Archivo")]
        [Required]
        public IFormFile Archivo { get; set; }

        [FromForm(Name = "Nombre")]
        [Required, MaxLength(255)]
        public string Nombre { get; set; }
    }
}
