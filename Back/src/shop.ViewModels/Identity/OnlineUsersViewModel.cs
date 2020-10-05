using System.Collections.Generic;
using shop.Entities.Identity;

namespace shop.ViewModels.Identity
{
    public class OnlineUsersViewModel
    {
        public List<User> Users { set; get; }
        public int NumbersToTake { set; get; }
        public int MinutesToTake { set; get; }
        public bool ShowMoreItemsLink { set; get; }
    }
}