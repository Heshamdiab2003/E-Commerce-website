using MegaMart.Models;

namespace MegaMart.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int TotalProducts { get; set; }
        public List<Order> RecentOrders { get; set; }
    }
} 