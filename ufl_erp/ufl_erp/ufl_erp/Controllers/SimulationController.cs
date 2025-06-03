using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ufl_erp.Data;
using ufl_erp.Models;

namespace ufl_erp.Controllers
{
    public class SimulationController : Controller
    {
        private readonly DataContext _context;
        private readonly Random _rand = new();

        public SimulationController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateSales()
        {
            var users = _context.Users.Include(u => u.Person).ToList();
            var people = _context.People.ToList();
            var products = _context.Products.Where(p => !p.IsDeleted).Include(p => p.Stocks).ToList();
            var branches = _context.Branches.ToList();

            if (!users.Any() || !people.Any() || !products.Any() || !branches.Any())
                return BadRequest("No hay suficientes datos para generar ventas.");

            for (int i = 0; i < 5; i++)
            {
                var user = users[_rand.Next(users.Count)];
                var person = people[_rand.Next(people.Count)];
                var branch = branches[_rand.Next(branches.Count)];

                var sale = new Sale
                {
                    UserId = user.Id,
                    PersonId = person.Id,
                    BranchId = branch.Id,
                    CreatedAt = DateTime.Now,
                    Status = 1,
                    NIT = $"NIT-{_rand.Next(100000, 999999)}",
                    IsDeleted = false,
                    Details = new List<Detail>()
                };

                int items = _rand.Next(1, 4); // Entre 1 a 3 productos por venta
                decimal total = 0;

                for (int j = 0; j < items; j++)
                {
                    var product = products[_rand.Next(products.Count)];
                    var quantity = _rand.Next(1, 5);
                    var price = product.Price;

                    sale.Details.Add(new Detail
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = price
                    });

                    total += quantity * price;
                }

                sale.Total = total;
                _context.Sales.Add(sale);
            }

            _context.SaveChanges();

            return Ok("Se generaron 5 ventas exitosamente.");
        }
    }
}
