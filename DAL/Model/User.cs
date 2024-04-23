﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class User : IdentityUser
    {
        #region Properties
        [Required]
        public string FullName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string PhoneNumber {  get; set; }
       
        public string? imageUrl { get; set; }
        public double MainLongitude { get; set; } = 0;
        public double MainLatitude { get; set; } = 0;
        
        public override string? UserName { get => base.UserName; set => base.UserName = value; }
        #endregion




        #region Navigations
        //public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
        #endregion
    }
}
