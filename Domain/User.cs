using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
namespace Domain;

public class User
{
    [Key]
    private int Id { set; get; }
    [Column(TypeName = "nvarchar(50)")]
    public string Name { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]
    public string EmailAddress { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]
    public Gender Gender { set; get; }
    
    [Column(TypeName = "date")]
    public DateOnly DateOfBirth { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]
    public string? Diet { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]
    public Address Address { set; get; }

    public User(string name, string emailAddress, Gender gender, DateOnly dateOfBirth, string diet, Address address)
    {
        Name = name;
        EmailAddress = emailAddress;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Diet = diet;
        Address = address;
    }
}
public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options):
        base(options){}
    
    public DbSet<User> Users { get; set; }
}