using Books.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.Replication.TestDecoding;
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
        private NpgsqlDataSource _dataSource;

        public BooksController(NpgsqlDataSource datasource, ILogger<BooksController> logger)
        {
            _dataSource = datasource;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<BookModel>> Get()
        {
            try
            {
                var result = new List<BookModel>();

                await using var command = _dataSource.CreateCommand("SELECT * FROM books");
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new BookModel
                        {
                            Name = reader.GetString(0),
                            chapter = reader.GetInt32(1),
                            comment = reader.GetString(2)
                        });
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<BookModel>();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookModel book)
        {
            try
            {
                var sql = @"INSERT INTO books (name, chapter, comment) VALUES(@name, @chapter, @comment)";
                await using var cmd = _dataSource.CreateCommand(sql);

                cmd.Parameters.AddWithValue("@name", book.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@chapter", book.chapter);
                cmd.Parameters.AddWithValue("@comment", book.comment ?? string.Empty);

                await cmd.ExecuteNonQueryAsync();

                _logger.LogInformation("A NEW BOOK HAS BEEN ADDED");

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
