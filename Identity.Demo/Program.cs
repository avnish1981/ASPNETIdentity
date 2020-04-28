using Microsoft.AspNet.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string userName = "avnish.choubey";
                string Password = "Password123!";
                string Email = "avnish.choubey@gmail.com";
                //var userStore = new UserStore<IdentityUser>();
                //var userManager = new UserManager<IdentityUser>(userStore);
                var userStore = new CustomerUserStore(new CustomerUserDbContext());
                var userManager = new UserManager<CustomerUser,int >(userStore);

                //user Creation..
                //var createUser = userManager.Create(new IdentityUser(userName), Password);
                //Console.WriteLine("Ceated {0}", createUser.Succeeded );
                //Console.ReadLine();
                //user Creation..
                var createResult = userManager.Create(new CustomerUser {UserName  = userName, Email=Email },Password ); 
                Console.WriteLine("Created {0}", createResult.Succeeded);
                Console.ReadLine();
                ////User Claim
                var user = userManager.FindByName(userName);
                //var claimResult = userManager.AddClaim(user.Id, new Claim("given_Name","avnish"));
                //Console.WriteLine("Claim {0)", claimResult.Succeeded);
                //Console.ReadLine();
                //Verify Password
                //bool isMatch = userManager.CheckPassword(new IdentityUser(userName), Password);
                //Console.WriteLine("Password Match {0}", isMatch);
                //Console.ReadLine();
                var checkpwd = userManager.CheckPassword(user, Password); 
                Console.WriteLine("Password Match {0}", checkpwd);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }


        }

        public class CustomerUser : IUser<int>
        {
            public int Id { get; set; }

            public string UserName { get; set; }
            public string PasswordHash { get; set; }
            public string Email { get; set; }
        }

        public class CustomerUserDbContext : DbContext
        {
            public CustomerUserDbContext() : base("DefaultConnection") { }
            public DbSet<CustomerUser> Users { get; set; }

            //Create DB Record..
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                var user = modelBuilder.Entity<CustomerUser>();
                user.ToTable("Users");
                user.HasKey(x => x.Id);
                user.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                user.Property(x => x.UserName).IsRequired().HasMaxLength(256)
                    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));
                
                base.OnModelCreating(modelBuilder);
            }
        }


        public class CustomerUserStore : IUserPasswordStore<CustomerUser, int>
        {
            private readonly CustomerUserDbContext _context;

            public CustomerUserStore(CustomerUserDbContext context )
            {
                this._context = context;
            }
            public Task CreateAsync(CustomerUser user)
            {
                _context.Users.Add(user);
                return _context.SaveChangesAsync();
            }

            public Task DeleteAsync(CustomerUser user)
            {
                _context.Users.Remove(user);
                return _context.SaveChangesAsync();
            }

            public void Dispose()
            {
                _context.Dispose();
            }

            public Task<CustomerUser> FindByIdAsync(int userId)
            {
                return _context.Users.FirstOrDefaultAsync (x => x.Id == userId);
            }

            public Task<CustomerUser> FindByNameAsync(string userName)
            {
                return _context.Users.FirstOrDefaultAsync(a => a.UserName == userName);
            }

            public Task<string> GetPasswordHashAsync(CustomerUser user)
            {
                return Task.FromResult(user.PasswordHash);
            }

            public Task<bool> HasPasswordAsync(CustomerUser user)
            {
                return Task.FromResult(user.PasswordHash != null);
            }

            public Task SetPasswordHashAsync(CustomerUser user, string passwordHash)
            {
                user.PasswordHash = passwordHash;
                return Task.CompletedTask;
            }

            public Task UpdateAsync(CustomerUser user)
            {
                _context.Users.Attach(user);
                return _context.SaveChangesAsync();
            }
        }






















    }
   
}
