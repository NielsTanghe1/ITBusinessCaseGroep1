using ITBusinessCase.Contracts;
using ITBusinessCase.Data;
using ITBusinessCase.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITBusinessCase.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IPublishEndpoint _publish;
    private readonly ApplicationDbContext _context;

    public OrdersController(IPublishEndpoint publish, ApplicationDbContext context)
    {
        _publish = publish;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new PlaceOrderViewModel
        {
            Items = BeanCatalog.Beans
                .Select(b => new BeanOrderItemInput
                {
                    BeanId = b.Id,
                    RoastedKg = 0,
                    GroundKg = 0,
                    RawKg = 0
                })
                .ToList()
        };

        // ✅ Geen UserManager => geen Identity EF query => geen "u.Id" SQLite error
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile != null)
            {
                vm.FirstName = profile.FirstName;
                vm.LastName = profile.LastName;
                vm.Email = profile.Email;
                vm.Street = profile.Street;
                vm.Postbus = profile.Postbus;
                vm.City = profile.City;
                vm.Postcode = profile.Postcode;
                vm.Country = profile.Country;
            }
            else
            {
                vm.Email = User.Identity?.Name ?? "";
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlaceOrderViewModel model)
    {
        model.Items ??= new List<BeanOrderItemInput>();

        if (!model.Items.Any(i => i.RoastedKg > 0 || i.GroundKg > 0 || i.RawKg > 0))
            ModelState.AddModelError("", "Kies minstens één producttype en zet KG groter dan 0.");

        if (!ModelState.IsValid)
        {
            // Zorg dat items niet verdwijnen als model invalid is
            if (model.Items.Count == 0)
            {
                model.Items = BeanCatalog.Beans
                    .Select(b => new BeanOrderItemInput
                    {
                        BeanId = b.Id,
                        RoastedKg = 0,
                        GroundKg = 0,
                        RawKg = 0
                    })
                    .ToList();
            }

            return View(model);
        }

        var beansById = BeanCatalog.Beans.ToDictionary(b => b.Id, b => b);
        var lines = new List<OrderLine>();

        foreach (var item in model.Items)
        {
            if (!beansById.TryGetValue(item.BeanId, out var bean))
                throw new Exception($"Onbekende beanId: {item.BeanId}");

            void AddLine(string type, int kg)
            {
                if (kg <= 0) return;

                var unit = BeanCatalog.GetPricePerKg(type, bean.Id);
                var lineTotal = unit * kg;

                lines.Add(new OrderLine(
                    BeanId: bean.Id,
                    BeanName: bean.Name,
                    ProductType: type,
                    Kg: kg,
                    UnitPricePerKg: unit,
                    LineTotal: lineTotal
                ));
            }

            AddLine("Roasted", item.RoastedKg);
            AddLine("Ground", item.GroundKg);
            AddLine("Raw", item.RawKg);
        }

        var total = lines.Sum(l => l.LineTotal);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var userName = User.Identity?.Name ?? "";

        var message = new OrderSubmitted(
            OrderId: Guid.NewGuid(),
            UserId: userId,
            UserName: userName,
            FirstName: model.FirstName ?? "",
            LastName: model.LastName ?? "",
            Email: model.Email ?? "",
            Street: model.Street ?? "",
            Postbus: model.Postbus ?? "",
            City: model.City ?? "",
            Postcode: model.Postcode ?? "",
            Country: model.Country ?? "",
            Total: total,
            Lines: lines
        );

        await _publish.Publish(message);

        // ✅ Opslaan/updaten van profiel voor volgende bestellingen
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { UserId = userId };
                _context.UserProfiles.Add(profile);
            }

            profile.FirstName = model.FirstName ?? "";
            profile.LastName = model.LastName ?? "";
            profile.Email = model.Email ?? "";
            profile.Street = model.Street ?? "";
            profile.Postbus = model.Postbus ?? "";
            profile.City = model.City ?? "";
            profile.Postcode = model.Postcode ?? "";
            profile.Country = model.Country ?? "";

            await _context.SaveChangesAsync();
        }

        TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
        return RedirectToAction(nameof(Create));
    }
}
