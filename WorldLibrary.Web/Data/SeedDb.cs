using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context,
            IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }
        public async Task SeedAsync() 
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");

            await _userHelper.CheckRoleAsync("Librarian");

            await _userHelper.CheckRoleAsync("Assistant");

            await _userHelper.CheckRoleAsync("Customer");

            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal"
                });

                await _context.SaveChangesAsync();
            }

            var userAdmin = await _userHelper.GetUserByEmailAsync("evelynnunes@yopmail.com");

            if (userAdmin == null)
            {
                userAdmin = new User
                {
                    FirstName = "Evelyn",
                    LastName = "Nunes",
                    Email = "evelynnunes@yopmail.com",
                    UserName = "evelynnunes@yopmail.com",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(userAdmin, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await _userHelper.AddUserToRoleAsync(userAdmin, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(userAdmin);
                await _userHelper.ConfirmEmailAsync(userAdmin, token);
            }
            var userLibrarian = await _userHelper.GetUserByEmailAsync("daianefarias@yopmail.com");

            if (userLibrarian == null)
            {
                userLibrarian = new User
                {
                    FirstName = "Daiane",
                    LastName = "Farias",
                    Email = "daianefarias@yopmail.com",
                    UserName = "daianefarias@yopmail.com",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()

                };

                var result = await _userHelper.AddUserAsync(userLibrarian, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await _userHelper.AddUserToRoleAsync(userLibrarian, "Librarian");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(userLibrarian);
                await _userHelper.ConfirmEmailAsync(userLibrarian, token);

            }
            var userAssistant = await _userHelper.GetUserByEmailAsync("romeupires@yopmail.com");

            if (userAssistant == null)
            {
                userAssistant = new User
                {
                    FirstName = "Romeu",
                    LastName = "Pires",
                    Email = "romeupires@yopmail.com",
                    UserName = "romeupires@yopmail.com",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()

                };
               
                var result = await _userHelper.AddUserAsync(userAssistant, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await _userHelper.AddUserToRoleAsync(userAssistant, "Assistant");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(userAssistant);
                await _userHelper.ConfirmEmailAsync(userAssistant, token);

            }
            var userCustomer = await _userHelper.GetUserByEmailAsync("mariaavelar@yopmail.com");
            if (userCustomer == null)
            {
                userCustomer = new User
                {
                    FirstName = "Maria",
                    LastName = "Avelar",
                    Email = "mariaavelar@yopmail.com",
                    UserName = "mariaavelar@yopmail.com",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(userCustomer, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await _userHelper.AddUserToRoleAsync(userCustomer, "Customer");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(userCustomer);
                await _userHelper.ConfirmEmailAsync(userCustomer, token);

            }
            var isInRoleAdmin = await _userHelper.IsUserInRoleAsync(userAdmin, "Admin");
            var isInRoleLibrarian = await _userHelper.IsUserInRoleAsync(userLibrarian, "Librarian");
            var isInRoleAssistant = await _userHelper.IsUserInRoleAsync(userAssistant, "Assistant");
            var isInRoleCustomer = await _userHelper.IsUserInRoleAsync(userCustomer, "Customer");


            if (!isInRoleAdmin)
            {
                await _userHelper.AddUserToRoleAsync(userAdmin, "Admin");

            }
            if (!isInRoleLibrarian)
            {
                await _userHelper.AddUserToRoleAsync(userLibrarian, "Librarian");

            }
            if (!isInRoleAssistant)
            {
                await _userHelper.AddUserToRoleAsync(userAssistant, "Assistant");

            }
            if (!isInRoleCustomer)
            {
                await _userHelper.AddUserToRoleAsync(userCustomer, "Customer");

            }

            if (!_context.Books.Any())
            {
                AddBook("Orgulho e preconceito", "Jane Austen", "1813",
                    "É uma história de amor poderosa, entre Elizabeth Bennet, a filha de espírito vivo e independente de um pequeno proprietário rural, " +
                    "e Mr. Darcy, um aristocrata altivo da mais antiga linhagem. Mas é também uma deliciosa comédia social, à qual estão subjacentes temáticas mais profundas.",
                     "romance", userAdmin);
                AddBook("O Conto da Aia", "Margaret Atwood", "1985", "Trata-se de um drama distópico passado nos Estados Unidos num " +
                    "contexto bastante catastrófico: um grupo de fundamentalistas religiosos consegue derrubar o governo e assume o poder fundando a República de Gilead.",
                     "Drama", userAdmin);
                AddBook("O Pequeno Príncipe", "Antoine de Saint - Exupéry", "1943",
                    "Conta a história da amizade entre um homem frustrado por ninguém compreender os seus desenhos, com um principezinho que habita um asteroide no espaço.",
                     "Literatura Infanto-Juvenil", userAdmin);
                AddBook("As Crônicas de Nárnia", "C. S. Lewis", "1950",
                    "A história começa na Segunda Guerra Mundial, quando quatro crianças (Pedro, Lúcia, Edmundo e Susana) são obrigadas a sair de Londres por causa dos bombardeios e irem para uma pequena cidade na Inglaterra, " +
                    "na casa de um professor solteiro",
                    "Adventure", userAdmin);
                AddBook("Guerra e Paz", "Leo Tolstói", "1985",
                    "Guerra e Paz é um verdadeiro monumento da literatura universal. Tolstói descreve as guerras movidas por Napoleão contra as principais monarquias da Europa, " +
                    "dissecando as origens e as consequências dos conflitos e, principalmente, expondo as pessoas e as suas vulnerabilidades com uma aguda perceção psicológica",
                    "Guerra", userAdmin);

                await _context.SaveChangesAsync();
            }
            if (!_context.Customers.Any())
            {
                AddCustomer("Arhur Reis", userLibrarian);
                AddCustomer("Clara Dias", userLibrarian);
                AddCustomer("Mariana Oliveira", userLibrarian);
                AddCustomer("Olivia Borba", userLibrarian);
                AddCustomer("Romeu Pires", userLibrarian);

                await _context.SaveChangesAsync();

            }
            if (!_context.Employees.Any())
            {
                AddEmployee("Beatriz Fonseca", "Librarian", userLibrarian);
                AddEmployee("Dinis Silva", "Assistant", userAssistant);
                AddEmployee("Filipa Correa", "Admin", userAdmin);
                AddEmployee("Maria Sousa", "Assistant", userLibrarian);
                AddEmployee("Rafael Santos", "Librarian", userLibrarian);

                await _context.SaveChangesAsync();

            }
            if (!_context.PhysicalLibraries.Any())
            {
                AddLibrary("Biblioteca Portugal", "Portugal", userAdmin);
                AddLibrary("Library of London", "London", userAdmin);
                AddLibrary("Biblioteca Espanã", "España", userAdmin);

                await _context.SaveChangesAsync();

            }


        }

        private void AddLibrary(string name, string country, User user)
        {
            _context.PhysicalLibraries.Add(new PhysicalLibrary
            {
                Name = name,
                Country = country,
                PhoneNumber = GenerateRandomNumbers(9),
                Email = name.Replace(" ", ".") + "@yopmail.com",
                User= user
            });
        }

        private void AddEmployee(string name, string jobPosition, User user)
        {
            _context.Employees.Add(new Employee
            {
                FullName= name,
                Document = GenerateRandomNumbers(9),
                Address= GenerateRandomAddress(),
                CellPhone= GenerateRandomNumbers(9),
                Email = name.Replace(" ", "_") + "@yopmail.com",
                JobPosition = jobPosition,
                User= user

            });

        }

        private void AddCustomer(string name, User user)
        {
            _context.Customers.Add(new Customer
            {
                FullName= name,
                Document= GenerateRandomNumbers(9),
                Address= GenerateRandomAddress(),
                Phone= GenerateRandomNumbers(9),
                Email= name.Replace(" ", "_") + "@yopmail.com",
                Premium = false,
                User= user

            });
        }

        private void AddBook(string title, string author, string year, string synopsis, string category, User user)
        {

            _context.Books.Add(new Book
            {
                Title = title,
                Author = author,
                Year = year,
                Synopsis = synopsis,
                Category = category,
                Assessment = "5 *",
                Quantity = Convert.ToDouble(GenerateRandomNumbers(1)),
                StatusBook = StatusBook.Available,
                User = user

            }) ;

        }
        private string GenerateRandomNumbers(int value)
        {
            string phoneNumber = "";
            for (int i = 0; i < value; i++)
            {
                phoneNumber += _random.Next(1, 10).ToString();
            }
            return phoneNumber;
        }
        private string GenerateRandomAddress()
        {
            string[] streetSuffixes = { "Rua", "Avenida", "Praceta", "Calçada" };
            string[] streets = { "José", "Brasil", "Angola", "Carlos", "Rodrigues", "Tody", "Santos", "Silva", "Conceição", "Teles" };
            string number = _random.Next(1, 1000).ToString();
            string streetSuffix = streetSuffixes[_random.Next(streetSuffixes.Length)];
            string street = streets[_random.Next(streets.Length)];

            return $" {streetSuffix}: {street}, {number}";
        }

    }
}
