using HomeLibrary.Application.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Linq;

namespace HomeLibrary.Application.Services.Models
{
    public class RawBook
    {
        public int Id { get; set; }

        public string Title { get; set; } = String.Empty;

        public int PublishYear { get; set; }

        public string Content { get; set; } = string.Empty;
        public string AuthorsXml { get; set; } = string.Empty;


        [NotMapped]
        public List<ChapterDto> Chapters
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AuthorsXml))
                    return new List<ChapterDto>();

                var doc = XDocument.Parse(Content);

                return doc.Root?
                    .Elements("Chapter")
                    .Select(a => new ChapterDto
                    {
                        Name = (string?) a.Element("Name").Value ?? string.Empty
                    })
                    .ToList()
                    ?? new List<ChapterDto>();

            }
        }

        [NotMapped]
        public List<AuthorDto> Authors
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AuthorsXml))
                    return new List<AuthorDto>();

                var doc = XDocument.Parse(AuthorsXml);

                return doc.Root?
                    .Elements("Author")
                    .Select(a => new AuthorDto
                    {
                        Id = (int?)a.Attribute("Id") ?? 0,
                        FirstName = (string?)a.Attribute("FirstName") ?? string.Empty,
                        LastName = (string?)a.Attribute("LastName") ?? string.Empty
                    })
                    .ToList()
                    ?? new List<AuthorDto>();
            }
        }

        public BookDto ToDto()
        {
            var result = new BookDto()
            {
                Id = Id,
                Title = Title,
                PublishYear = PublishYear,
                Authors = Authors,
                Chapters = Chapters
            };

            return result;
        }
    }
}
