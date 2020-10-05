using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace shop.ViewModels.Reservation
{
    public class ClaimViewModel 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
    public class RoleClaimViewModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
    public class ClaimCustomeViewModel
    {
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}