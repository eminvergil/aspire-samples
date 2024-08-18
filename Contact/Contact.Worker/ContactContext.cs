using Contact.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace Contact.Worker;

public class ContactContext : DbContext
{
    public ContactContext(DbContextOptions<ContactContext> options) : base(options)
    {
    }

    public DbSet<ContactModel> Contacts { get; set; }
}
