﻿using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.ApiUser
{
    public class ApiUserDto : LoginDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        

    }
}