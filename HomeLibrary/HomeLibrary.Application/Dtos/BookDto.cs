using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HomeLibrary.Application.Dtos
{
    public class BookDto
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlIgnore]
        public int PublishYear { get; set; }

        [XmlIgnore]
        public string Title { get; set; } = string.Empty;

        [XmlArray("Chapters")]
        public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();

        [XmlIgnore]
        public IList<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    }
}
