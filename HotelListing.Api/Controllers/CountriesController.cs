using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Country;
using AutoMapper;
using HotelListing.Api.Contracts;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICountriesRepository countriesRepository;

        public CountriesController( IMapper mapper, ICountriesRepository _countriesRepository)
        {
            _mapper = mapper;
            this.countriesRepository = _countriesRepository;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await countriesRepository.GetAllAsync();
            var countryList = _mapper.Map<List<CountryDto>>(countries);
            return Ok(countryList);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDto>> GetCountry(int id)
        {
            //var countryModel = await _context.Countries.Include(x=>x.Hotels).FirstOrDefaultAsync(y=> y.Id == id);
            var countryModel = await countriesRepository.GetDetails(id);

            if (countryModel == null)
            {
                return NotFound();
            }
            var country = _mapper.Map<CountryDetailsDto>(countryModel);
            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updatecountryDtocountry)
        {
            if (id != updatecountryDtocountry.Id)
            {
                return BadRequest();
            }

            // _context.Entry(country).State = EntityState.Modified;

           // var country = await _context.Countries.FindAsync(id);
           var country = await countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            _mapper.Map(updatecountryDtocountry, country);
            try
            {
                await countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
        { 
        
           // var countryOld = new Country() { Name = createCountry.Name,ShortName=createCountry.ShortName };
            var country=_mapper.Map<Country>(createCountry);

           // _context.Countries.Add(country);
          //  await _context.SaveChangesAsync();
          await countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            //var country = await _context.Countries.FindAsync(id);
            var country = await countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }

           // _context.Countries.Remove(country);
           // await _context.SaveChangesAsync();

            await countriesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async  Task<bool> CountryExists(int id)
        {
            //return _context.Countries.Any(e => e.Id == id);
           return await countriesRepository.Exists(id);
        }
    }
}
