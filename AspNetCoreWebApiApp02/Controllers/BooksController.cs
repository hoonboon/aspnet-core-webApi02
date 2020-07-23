using AspNetCoreWebApiApp02.Models;
using AspNetCoreWebApiApp02.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreWebApiApp02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly MongoDbService _dbService;
        private readonly BookService _bookService;

        public BooksController(MongoDbService dbService, BookService bookService)
        {
            _dbService = dbService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAsync() =>
            await _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public async Task<ActionResult<Book>> GetAsync(string id)
        {
            var book = await _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            await _bookService.Create(book);

            return CreatedAtRoute("GetBook", new { id = book.Id.ToString() }, book);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Book bookIn)
        {
            using (var session = await _dbService.StartSessionAsync())
            {
                try
                {
                    session.StartTransaction();

                    var book = await _bookService.Get(session, id);

                    if (book == null)
                    {
                        return NotFound();
                    }

                    _bookService.Update(session, book.Id, bookIn);
                }
                catch
                {
                    await session.AbortTransactionAsync();
                    throw;
                }
                await session.CommitTransactionAsync();
            }

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Remove(book.Id);

            return NoContent();
        }
    }
}
