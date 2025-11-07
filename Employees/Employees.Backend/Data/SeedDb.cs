using Employees.Backend.UnitsOfWork.Interfaces;
using Employees.Shared.Entities;
using Employees.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Employees.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesFullAsync();
        await CheckEmployeesFullAsync();
        await CheckRolesAsync();
        await CheckUserAsync("2010", "Kevin", "Jaramillo", "kevinney10@hotmail.com", "3006889085", "Cr 65# 95-95", UserType.Admin);
    }

    private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Address = address,
                Document = document,
                City = _context.Cities.FirstOrDefault(),
                UserType = userType,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());
        }

        return user;
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task CheckEmployeesFullAsync()
    {
        if (!_context.Employees.Any())
        {
            var employeesSQLScript = File.ReadAllText("Data\\EmployeesTaskScript.sql");
            await _context.Database.ExecuteSqlRawAsync(employeesSQLScript);
        }
    }

    private async Task CheckCountriesFullAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\CountriesStatesCities.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }
}