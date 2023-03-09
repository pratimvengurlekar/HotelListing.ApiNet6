using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Models.Hotel;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelsRepositoty _hotelsRepositoty;

        public HotelsController(IMapper mapper,IHotelsRepositoty hotelsRepositoty)
        {
            this._mapper = mapper;
            this._hotelsRepositoty = hotelsRepositoty;

            //_context = context;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            //return await _context.Hotels.ToListAsync();
            var hotels = await _hotelsRepositoty.GetAllAsync();

            return Ok(_mapper.Map<List<HotelDto>>(hotels));
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            // var hotel = await _context.Hotels.FindAsync(id
            var hotel = await _hotelsRepositoty.GetAsync(id);
       


            if (hotel == null)
            {
                return NotFound();
            }

            var hoteldto = _mapper.Map<HotelDto>(hotel);

            return Ok(hoteldto);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto updateHotelDto)
        {
            if (id != updateHotelDto.Id)
            { 
                return BadRequest();
            }
            var hotel = await _hotelsRepositoty.GetAsync(id);
            if(hotel == null)
            {
                return NotFound();
            }

            //_context.Entry(hotel).State = EntityState.Modified;
            _mapper.Map(updateHotelDto, hotel);

            try
            {
                await _hotelsRepositoty.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
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

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hoteldto)
        {
            var hotel = _mapper.Map<Hotel>(hoteldto);

            //_context.Hotels.Add(hotel);
            await _hotelsRepositoty.AddAsync(hotel);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotelsRepositoty.GetAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            // _context.Hotels.Remove(hotel);
            //  await _context.SaveChangesAsync();
            await _hotelsRepositoty.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            //return _context.Hotels.Any(e => e.Id == id);
            return await _hotelsRepositoty.Exists(id);
        }
    }
}
