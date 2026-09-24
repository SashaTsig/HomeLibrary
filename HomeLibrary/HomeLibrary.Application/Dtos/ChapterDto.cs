using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HomeLibrary.Application.Dtos
{
    [XmlType("Chapter")]
    public class ChapterDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
