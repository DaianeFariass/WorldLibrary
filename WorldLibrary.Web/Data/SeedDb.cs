using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;

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
            await _context.Database.EnsureCreatedAsync();

            var userAdmin = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");

            if (userAdmin == null)
            {
                userAdmin = new User
                {
                    FirstName = "Evelyn",
                    LastName = "Nunes",
                    Email = "evelyn.nunes@cinel.pt",
                    UserName = "evelyn.nunes@cinel.pt",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                };

                var result = await _userHelper.AddUserAsync(userAdmin, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

            }
            var userEmployee = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");

            if (userEmployee == null)
            {
                userEmployee = new User
                {
                    FirstName = "Daiane",
                    LastName = "Farias",
                    Email = "daiane.farias@cinel.pt",
                    UserName = "daiane.farias@cinel.pt",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),

                };

                var result = await _userHelper.AddUserAsync(userEmployee, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

            }
            var userCustomer = await _userHelper.GetUserByEmailAsync("livania.viegas@cinel.pt");
            if (userCustomer == null)
            {
                userCustomer = new User
                {
                    FirstName = "Livânia",
                    LastName = "Viegas",
                    Email = "livania.viegas@cinel.pt",
                    UserName = "livania.viegas@cinel.pt",
                    PhoneNumber = GenerateRandomNumbers(9),
                    Address = GenerateRandomAddress(),
                };

                var result = await _userHelper.AddUserAsync(userCustomer, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

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
                AddCustomer("Arhur Reis", userCustomer);
                AddCustomer("Clara Dias", userCustomer);
                AddCustomer("Mariana Oliveira", userCustomer);
                AddCustomer("Olivia Borba", userCustomer);
                AddCustomer("Romeu Pires", userCustomer);

                await _context.SaveChangesAsync();

            }
            if (!_context.Employees.Any())
            {
                AddEmployee("Beatriz Fonseca", "Bibliotecario", userEmployee);
                AddEmployee("Dinis Silva", "Gerente", userEmployee);
                AddEmployee("Filipa Correa", "Assistente", userEmployee);
                AddEmployee("Maria Sousa", "Interno", userEmployee);
                AddEmployee("Rafael Santos", "Bibliotecario", userEmployee);

                await _context.SaveChangesAsync();

            }
            if (!_context.PhysicalLibraries.Any())
            {
                AddLibrary("Biblioteca Nacional", "Portugal", userAdmin);
                AddLibrary("Nacional Library", "London", userAdmin);
                AddLibrary("Biblioteca Nacional", "España", userAdmin);

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
                Email = name.Replace(" ", ".") + "@cinel.com",
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
                Email = name.Replace(" ", "_") + "@cinel.com",
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
                Email= name.Replace(" ", "_") + "@cinel.com",
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
                User= user

            });

        }
        private string GenerateRandomNumbers(int value)
        {
            string phoneNumber = "";
            for (int i = 0; i < value; i++)
            {
                phoneNumber += _random.Next(10).ToString();
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
