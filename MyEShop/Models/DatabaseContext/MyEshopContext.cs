using Microsoft.EntityFrameworkCore;
using MyEShop.Models.Entities;
using MyEShop.Pages.Admin;

namespace MyEShop.Models.DatabaseContext
{
    public class MyEshopContext :DbContext
    {
        public MyEshopContext(DbContextOptions<MyEshopContext> options) : base(options)
        {

        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryToProduct> CategoryToProducts { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Seed Data Category
            modelBuilder.Entity<Category>()
                .HasData(
                new Category { Id = 1, Name = "ASP.NET", Description = "آموزش asp.net core" },
                new Category { Id = 2, Name = "ساعت مچی", Description = "ساعت مچی" },
                new Category { Id = 3, Name = "لباس ورزش", Description = "لباس ورزش" },
                new Category { Id = 4, Name = "لوازم منزل", Description = "لوازم منزل" }
                );
            #endregion

            modelBuilder.Entity<CategoryToProduct>()
                .HasKey(s => new { s.CategoryId, s.ProductId });

            modelBuilder.Entity<CategoryToProduct>()
                .HasOne(x => x.Category)
                .WithMany(x => x.CategoryToProduct)
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<CategoryToProduct>()
                .HasOne(x => x.Product)
                .WithMany(x => x.CategoryToProduct)
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne<Item>(s => s.Item)
                .WithOne(s => s.Product)
                .HasForeignKey<Item>(s => s.Id);

            modelBuilder.Entity<Item>()
                .Property(a => a.Price)
                .HasColumnType("Money");

            modelBuilder.Entity<Item>()
                 .HasData(
                    new Item { Id = 1 , Price = 854.0M , QuantityInStock =5 },
                    new Item { Id = 2, Price = 3302.0M , QuantityInStock =8 },
                    new Item { Id = 3, Price =2500 , QuantityInStock =3 }
                );
            modelBuilder.Entity<Product>()
                 .HasData(
                    new Product { Id = 1, ItemId =1  , Name = "آموزش Asp.net core", 
                        Description= "Lorem ipsum dolor sit amet consectetur adipisicing elit." +
                        " Assumenda odit, rerum nulla temporibus laudantium nisi unde? Quo, quaerat." +
                        " Dolore ex cupiditate saepe facilis et at velit error magnam earum molestias!" },
                    new Product { Id = 2, ItemId =2 , Name ="آموزش blazor از مقدماتی تا پیشرفته" ,
                        Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit." +
                        " Assumenda odit, rerum nulla temporibus laudantium nisi unde? Quo, quaerat." +
                        " Dolore ex cupiditate saepe facilis et at velit error magnam earum molestias!" },
                    new Product { Id = 3, ItemId = 3, Name = "آموزش اپلیکیشن های پیشرونده",
                        Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit." +
                        " Assumenda odit, rerum nulla temporibus laudantium nisi unde? Quo, quaerat." +
                        " Dolore ex cupiditate saepe facilis et at velit error magnam earum molestias!" }
                );
            modelBuilder.Entity<CategoryToProduct>()
                 .HasData(
                    new CategoryToProduct { CategoryId = 1 ,ProductId= 1  },
                    new CategoryToProduct { CategoryId = 2,ProductId= 1  },
                    new CategoryToProduct { CategoryId = 3,ProductId=1   },
                    new CategoryToProduct { CategoryId = 4,ProductId=1   },
                    new CategoryToProduct { CategoryId = 1 ,ProductId= 2  },
                    new CategoryToProduct { CategoryId = 2,ProductId= 2  },
                    new CategoryToProduct { CategoryId = 3,ProductId=2   },
                    new CategoryToProduct { CategoryId = 4,ProductId=2   },
                    new CategoryToProduct { CategoryId = 1 ,ProductId= 3  },
                    new CategoryToProduct { CategoryId = 2,ProductId= 3  },
                    new CategoryToProduct { CategoryId = 3,ProductId=3   },
                    new CategoryToProduct { CategoryId = 4,ProductId=3   }
                   
                );
                 
            modelBuilder.Entity<Cart>()
                .HasKey(c=>c.OrderId);
        }
        public DbSet<MyEShop.Pages.Admin.ProductViewModel> ProductViewModel { get; set; } = default!;
    }   
}
