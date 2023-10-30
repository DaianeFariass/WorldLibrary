using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Método que adiciona a cidade 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>City</returns>
        public async Task AddCityAsync(CityViewModel model)
        {
            var country = await this.GetCountryWithCitiesAsync(model.CountryId);
            if (country == null)
            {
                return;
            }

            country.Cities.Add(new City { Name = model.Name });
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Métodod que permite deletar a cidade de uma determinado país
        /// </summary>
        /// <param name="city"></param>
        /// <returns>Country.Id</returns>
        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await _context.Countries
               .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
               .FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return country.Id;
        }

        public async Task<IEnumerable<City>> GetAllCities()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        /// <summary>
        /// Método que preenche a combobox das cidades 
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>Cities</returns>
        public IEnumerable<SelectListItem> GetComboCities(int countryId)
        {
            var country = _context.Countries.Find(countryId);
            var list = new List<SelectListItem>();
            if (country != null)
            {
                list = _context.Cities.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),

                }).OrderBy(l => l.Text).ToList();
                list.Insert(0, new SelectListItem
                {
                    Text = "Select a citie...",
                    Value = "0"

                });
            }

            return list;
        }

        /// <summary>
        /// Método que preenche a combobox dos paises 
        /// </summary>
        /// <returns>Country</returns>
        public IEnumerable<SelectListItem> GetComboCountries()
        {
            var list = _context.Countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),

            }).OrderBy(l => l.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "Select a country...",
                Value = "0"

            });

            return list;
        }

        /// <summary>
        /// Método que organiza as cidades e os paises por ordem alfabética
        /// </summary>
        /// <returns></returns>
        public IQueryable GetCountriesWithCities()
        {
            return _context.Countries
          .Include(c => c.Cities)
          .OrderBy(c => c.Name);
        }

        /// <summary>
        /// Método que busca os paises 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryAsync(City city)
        {
            return await _context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Método que mostra os paises e as cidades 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<Country> GetCountryWithCitiesAsync(int id)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Métodod que atualia a lista de cidades
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await _context.Countries
               .Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
            return country.Id;
        }
    }
}
