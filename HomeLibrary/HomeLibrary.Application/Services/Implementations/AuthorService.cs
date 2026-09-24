using HomeLibrary.Application.Dtos;
using HomeLibrary.Application.Services.Interfaces;
using HomeLibrary.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Application.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly HomeLibraryDbContext _dbContext;

        public AuthorService(HomeLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<AuthorDto>> GetAll(CancellationToken cancellationToken)
        {
            var authors = await _dbContext.Authors
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();

            return authors.Select(a => new AuthorDto()
            { 
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName
            }).ToList();

        }
    }
}
