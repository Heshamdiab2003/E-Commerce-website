using Microsoft.EntityFrameworkCore;
namespace Digital_Products.Data
{
    public class AppDbcontext  : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext>options) : base (options) 
        {
            
        }
    }
}
