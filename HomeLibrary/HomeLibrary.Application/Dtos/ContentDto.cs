using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HomeLibrary.Application.Dtos
{
    [XmlRoot("Chapters")]
    public class ContentDto
    {
        [XmlElement("Chapter")]
        public List<ChapterDto> Items { get; set; } = new();
    }
}
