using Books.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.ComponentModel;
using System.Net;
using BookModel = Books.Shared.Models.Books;

namespace Books.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<string>> Get()
        {

            try
            {
                var result = new List<string>();

                //at some point - put this in a config file
                var connectionString = "Host=172.167.22.253:5432;Username=hurew6shw6y329uehwsjq;Password=deuigdyw82wjia;Database=Books";
                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                await using var command = dataSource.CreateCommand("SELECT * FROM books");
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader.GetString(0));
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<string>();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookModel book)
        {
            try
            {
                //var book = new Books ("Case File Compedium", 111, "something something He Yu being insane");
                var connectionString = "Host=172.167.22.253:5432;Username=hurew6shw6y329uehwsjq;Password=deuigdyw82wjia;Database=Books";
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
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}
