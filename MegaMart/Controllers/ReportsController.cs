using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MegaMart.Models;
using MegaMart.Data;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MegaMart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(AppDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var viewModel = new ReportsViewModel
            {
                TotalSales = await _context.Orders.SumAsync(o => o.TotalAmount),
                MonthlySales = await _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth)
                    .SumAsync(o => o.TotalAmount),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Pending),
                ProcessingOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Processing),
                ShippedOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Shipped),
                DeliveredOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Cancelled),
                TopProducts = await GetTopProducts(5),
                OrderStatuses = await GetOrderStatuses(),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var sales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportViewModel
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            return View(sales);
        }

        public async Task<IActionResult> ProductReport()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.OrderItems)
                .Select(p => new ProductReportViewModel
                {
                    Product = p,
                    TotalQuantity = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .ToListAsync();

            return View(products);
        }

        private async Task<List<ProductReportViewModel>> GetTopProducts(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.OrderItems)
                .Select(p => new ProductReportViewModel
                {
                    Product = p,
                    TotalQuantity = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(count)
                .ToListAsync();
        }

        private async Task<Dictionary<OrderStatus, int>> GetOrderStatuses()
        {
            return await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }
    }

    public class ReportsViewModel
    {
        public decimal TotalSales { get; set; }
        public decimal MonthlySales { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public List<ProductReportViewModel> TopProducts { get; set; }
        public Dictionary<OrderStatus, int> OrderStatuses { get; set; }
        public List<Order> RecentOrders { get; set; }
    }

    public class ProductReportViewModel
    {
        public Product Product { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SalesReportViewModel
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
    }
} 