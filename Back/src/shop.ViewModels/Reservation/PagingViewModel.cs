using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shop.Entities.Reservation;
using shop.ViewModels;

namespace shop.ViewModels.Reservation
{
  
    public class PagingViewModel<T> where T:class
    {
        public List<T> Pages { get; set; } = new List<T>();
        public int TotalPage { get; set; }
        public int Count { get; set; }
    }
}