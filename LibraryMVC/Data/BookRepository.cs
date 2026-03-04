using System.Data;
using System.Data.SqlClient;
using Dapper;
using LibraryMVC.Models;
using Microsoft.Extensions.Configuration;

namespace LibraryMVC.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public BookRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Book>(
                "usp_SelectBooks",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var books = await connection.QueryAsync<Book>(
                "usp_SelectBooks",
                new { BookID = id },
                commandType: CommandType.StoredProcedure);
            return books.FirstOrDefault();
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "usp_InsertBook",
                new
                {
                    book.Title,
                    book.Author,
                    book.Year,
                    book.Genre,
                    book.Pages,
                    book.ISBN,
                    SchemaXml = book.SchemaXml ?? (object)DBNull.Value
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteAsync(
                "usp_UpdateBook",
                new
                {
                    book.BookID,
                    book.Title,
                    book.Author,
                    book.Year,
                    book.Genre,
                    book.Pages,
                    book.ISBN,
                    SchemaXml = book.SchemaXml ?? (object)DBNull.Value
                },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<bool> DeleteBookAsync(int id)  // ✅ Этот метод должен быть!
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteAsync(
                "usp_DeleteBook",
                new { BookID = id },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }

    }
}