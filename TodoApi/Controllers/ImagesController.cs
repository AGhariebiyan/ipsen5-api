using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GMAPI.Dtos;
using GMAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly PostgresDatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        
        public ImagesController(IMapper mapper, PostgresDatabaseContext context, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _context = context;
            _hostingEnvironment = environment;
        }

        // api/Images/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageById(Guid id)
        {
            var filename = Directory.EnumerateFiles(_hostingEnvironment.ContentRootPath +"/Images").SingleOrDefault(f => f.Contains(id + "."));
            var file = PhysicalFile(filename, "image/jpeg");
            return file;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Image>> DeleteImageById(Guid id)
        {
            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            _context.Image.Remove(image);
            await _context.SaveChangesAsync();

            return image;
        }
        
        [HttpPost]
        public async Task<ActionResult<Image>> SetPicture([FromForm] IFormFile picture)
        {
            if (picture.Length > 800000)
            {
                return BadRequest("image file too large");
            }
            var image = new Image();
            image.Id = Guid.NewGuid();
            var uploads = Path.Combine("Images", image.Id + "." + picture.FileName.Split('.').Last());
            using (var fileStream = new FileStream(uploads, FileMode.Create)) {
                await picture.CopyToAsync(fileStream);
            }
            image.Location = uploads;
            image.Url = "api/Images/" + image.Id;

            await _context.Image.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }
    }
}