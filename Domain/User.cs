using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
namespace Domain;
using Microsoft.AspNetCore.Identity;
public class User : IdentityUser
{
    public User() { }
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Diet { get; set; }
    public int? AddressId { get; set; }
    public Address Address { get; set; }

    public User(string name, string email, Gender gender, DateOnly dateOfBirth, string diet, int? addressId)
    {
        Name = name;
        Email = email;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Diet = diet;
        AddressId = addressId;
    }
}
