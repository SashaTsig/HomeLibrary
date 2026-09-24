using HomeLibrary.Application.Dtos;
using HomeLibrary.Application.Services.Interfaces;
using HomeLibrary.Application.Services.Models;
using HomeLibrary.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Serialization;

namespace HomeLibrary.Application.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly HomeLibraryDbContext _dbContext;

        public BookService(HomeLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> InsertBook(BookDto book, CancellationToken cancellationToken)
        {
            var tvp = AuthorsToSqlVar(book.Authors);

            var content = SerializeContent(book);

            int newBookId = 0;
            try
            {
                var result =  await _dbContext.Database
                .SqlQueryRaw<int>(
                    "EXEC dbo.SP_Books_Insert " +
                    "@Title = {0}, @PublishYear = {1}, @Content = {2}, @Authors = {3}, @NewBookId = {4} OUTPUT",
                    book.Title, book.PublishYear, content, tvp, newBookId)
                .ToListAsync(cancellationToken);

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BookDto?> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Database
                .SqlQueryRaw<RawBook>("EXEC dbo.SP_Books_GetById @Id = {0}", id)
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault()?.ToDto() ?? null;
        }

        public async Task<IList<BookDto>> Find(string? title, string? author, string? content, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Database
                .SqlQueryRaw<RawBook>("EXEC dbo.SP_Books_Search @Title = {0}, @Author={1}, @ConentTerm={2}", title, author, content)
                .ToListAsync(cancellationToken);

            return result.Select(r => r.ToDto()).ToList();
        }

        public async Task<BookDto> Update(int id, BookDto dto, CancellationToken cancellationToken)
        {
            var tvp = AuthorsToSqlVar(dto.Authors);

            var result = await _dbContext.Database
                .SqlQueryRaw<int>(
                    "EXEC dbo.SP_Books_Update " +
                    "@Id = {4}, @Title = {0}, @PublishYear = {1}, @Content = {2}, @Authors = {3}",
                    dto.Title, dto.PublishYear, SerializeContent(dto), tvp, dto.Id)
                .ToListAsync(cancellationToken);

            return dto;
        }

        private SqlParameter AuthorsToSqlVar(ICollection<AuthorDto> authors)
        {
            var authorsTable = new DataTable();

            authorsTable.Columns.Add("Id", typeof(int));
            authorsTable.Columns.Add("FirstName", typeof(string));
            authorsTable.Columns.Add("LastName", typeof(string));

            foreach (var author in authors)
            {
                authorsTable.Rows.Add(author.Id, author.FirstName, author.LastName);
            }

            return new SqlParameter("@Authors", SqlDbType.Structured)
            {
                TypeName = "dbo.AuthorList",
                Value = authorsTable
            };
        }

        private string SerializeContent(BookDto book)
        {
            var serializer = new XmlSerializer(typeof(ContentDto));

            using var sw = new StringWriter();

            var content = new ContentDto()
            {
                Items = book.Chapters
            };

            serializer.Serialize(sw, content);

            return sw.ToString();
        }
    }
}
