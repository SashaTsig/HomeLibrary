using HomeLibrary.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Application.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IList<AuthorDto>> GetAll(CancellationToken cancellationToken);
    }
}
