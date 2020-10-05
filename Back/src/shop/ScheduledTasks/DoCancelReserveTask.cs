using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTScheduler.Core.Contracts;
using Microsoft.Extensions.Logging;
using shop.Common.Notification;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Extension;
using shop.Services.Contracts.Reservation;

namespace shop.ScheduledTasks
{
    public class DoCancelReserveTask : IScheduledTask
    {
        private readonly ILogger<DoCancelReserveTask> _logger;
        private readonly IUserDeviceService _userDeviceService;
        private readonly IOrderService _orderService;
        public DoCancelReserveTask(ILogger<DoCancelReserveTask> logger,
                                 IUserDeviceService userDeviceService,
                                 IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        public bool IsShuttingDown { get; set; }

        public async Task RunAsync()
        {
            ///<summary>
            /// باید زمان رزرو چه کسانی به اتمام رسیده
            /// اگر وضعیت پرداختش موفق نبود و رزرو شده بود
            /// تعداد کم شده، به محصول اضافه شود
            ///</summary>
            if (this.IsShuttingDown)
                return;

            // await _orderService.CancellReserve();
           
             await Task.CompletedTask;
        }
    }
}