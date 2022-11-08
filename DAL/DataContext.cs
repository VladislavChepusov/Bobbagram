using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }
        //Модификации (Поле Емеил доожно быть уникальным)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder.Entity<Avatar>().ToTable(nameof(Avatars));
        }

        // Указывает где у нас будут прописываться миграции (API>Migrations)
        // скачать пакет npgsql для миграций
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("Api")); 

        // Оповещаем об появлении новых таблиц
        public DbSet<User> Users => Set <User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        // DbSet<UserPost> UserPosts => Set<UserPost>();
      //  public DbSet<Comment> Comments => Set<Comment>();
       // public DbSet<PostContent> PostContents => Set<PostContent>();

    }
}
