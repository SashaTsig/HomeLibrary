using HomeLibrary.Application.Dtos;
using HomeLibrary.Application.Services.Implementations;
using HomeLibrary.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] BookDto dto, CancellationToken cancellationToken)
        {
            var result = await _bookService.InsertBook(dto, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _bookService.GetById(id, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<BookDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Find(string? title, string? content, string? author, CancellationToken cancellationToken)
        {
            var result = await _bookService.Find(title, author, content, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] BookDto dto, CancellationToken cancellationToken)
        {
            var result = await _bookService.Update(id, dto, cancellationToken);

            return Ok(result);
        }
    }
}
