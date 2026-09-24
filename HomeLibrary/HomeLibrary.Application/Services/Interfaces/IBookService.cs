using HomeLibrary.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<int> InsertBook(BookDto book, CancellationToken cancellationToken);

        Task<BookDto> GetById(int id, CancellationToken cancellationToken);

        Task<IList<BookDto>> Find(string? title, string? author, string? content, CancellationToken cancellationToken);

        Task<BookDto> Update(int id, BookDto dto, CancellationToken cancellationToken);
    }
}
