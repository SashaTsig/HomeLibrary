using HomeLibrary.Application.Dtos;
using HomeLibrary.Application.Services.Interfaces;
using HomeLibrary.Application.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IList<AuthorDto>> GetAll(CancellationToken cancellationToken)
        {
            return await _authorService.GetAll(cancellationToken);
        }
    }
}
