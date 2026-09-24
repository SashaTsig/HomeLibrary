using HomeLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace HomeLibrary.Core
{
    public class HomeLibraryDbContext : DbContext
    {
        public virtual DbSet<Author> Authors { get; set; }

        public virtual DbSet<Book> Books { get; set; }

        public HomeLibraryDbContext(DbContextOptions<HomeLibraryDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);

            builder.Entity<Author>()
               .Property(a => a.FirstName)
               .HasMaxLength(50)
               .IsRequired();

            builder.Entity<Author>()
               .Property(a => a.LastName)
               .HasMaxLength(50)
               .IsRequired();

            builder.Entity<Book>()
               .Property(b => b.Title)
               .HasMaxLength(255)
               .IsRequired();

            builder.Entity<Book>()
                .Property(b => b.Content)
                .IsRequired()
                .HasColumnType("xml");

            builder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity(j => j.ToTable("BookAuthors"));

            builder.Entity<Author>()
                .HasIndex(a => new { a.LastName, a.FirstName })
                .HasDatabaseName("IX_Authors_LastName_FirstName");

            builder.Entity<Book>()
                .HasIndex(b => b.Title)
                .HasDatabaseName("IX_Books_Title");

        }
    }
}
