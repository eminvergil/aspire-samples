using Contact.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Contact.Api;

public class ContactContext : DbContext
{
    public ContactContext(DbContextOptions<ContactContext> options, IConfiguration configuration) : base(options)
    {
    }

    public DbSet<ContactModel> Contacts { get; set; }
}
