using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Core.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = String.Empty;

        public string LastName { get; set; } = String.Empty;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
