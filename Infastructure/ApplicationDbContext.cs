using iEvent.Domain;
using iEvent.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>, IUnitOfWork
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> TableEvents { get; set; }
        public DbSet<MapOfEvent> mapOfEvents { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemComment> ProblemComments { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<EventMarkUser> EventMarkUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public void Commit()
        {
            SaveChanges();
        }
    }
}