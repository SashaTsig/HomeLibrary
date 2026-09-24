using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Core.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = String.Empty;

        public int PublishYear { get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();

        //оглавление
        public string Content { get; set; } = String.Empty;
    }
}
