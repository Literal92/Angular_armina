using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
namespace shop.ViewModels.Reservation
{
    /// <summary>
    /// از نمونه کد زرین پال برداشتم
    /// </summary>

    public class PaymentResultViewModel
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public string TransactionCode { get; set; } = "";
        public string Id { get; set; } = "";


    }

}
