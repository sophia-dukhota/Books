using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BookModel = Books.Shared.Models.Books;

namespace Books.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IConfiguration _configuration;

        public BooksController(ILogger<BooksController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAsync()
        {

            try
            {
                var result = new List<string>();

                var connectionString = GetConnectionString();
                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                await using var command = dataSource.CreateCommand("SELECT * FROM books");
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader.GetString(0));
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch books.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Unable to fetch books.");
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BookModel book)
        {
            if (book is null)
            {
                return BadRequest("Book payload is required.");
            }

            if (string.IsNullOrWhiteSpace(book.Name))
            {
                return BadRequest("Book name is required.");
            }

            try
            {
                var connectionString = GetConnectionString();
                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                var sql = @"INSERT INTO books (name, chapter, comment) VALUES(@name, @chapter, @comment)";
                await using var cmd = dataSource.CreateCommand(sql);

                cmd.Parameters.AddWithValue("@name", book.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@chapter", book.chapter);
                cmd.Parameters.AddWithValue("@comment", book.comment);

                await cmd.ExecuteNonQueryAsync();

                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert book {@Book}", book);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to save book.");
            }
        }

        private string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("Books");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'Books' is not configured.");
            }

            return connectionString;
        }
    }
}
