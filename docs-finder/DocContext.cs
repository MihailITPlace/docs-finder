using System;
using docs_finder.models;
using Microsoft.EntityFrameworkCore;

namespace docs_finder
{
    public class DocContext : DbContext
    {
        public DbSet<DateOfLastMessage> Dates { get; set; }
        public DbSet<Doc> Docs { get; set; }

        public DocContext() => Database.EnsureCreated();
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=docs.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DateOfLastMessage>().HasData(new DateOfLastMessage {Id = 1, Date = DateTime.MinValue});
        }
    }
}