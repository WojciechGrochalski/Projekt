using angularapi.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularApi.DataBase
{
    public class CashDBContext : DbContext
    {

        public CashDBContext(DbContextOptions<CashDBContext> options) : base(options)
        {

        }
        public DbSet<CurrencyDBModel> cashDBModels { get; set; }
        public DbSet<UserDBModel> userDBModels { get; set; }
        public DbSet<Remainder> Remainders { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
    }
}
