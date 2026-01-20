using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

public class PaymentDetailsController : Controller {
	private readonly LocalDbContext _context;

	public PaymentDetailsController(LocalDbContext context) {
		_context = context;
	}

	// GET: PaymentDetails
	public async Task<IActionResult> Index() {
		var localDbContext = _context.PaymentDetails.Include(p => p.CoffeeUser);
		return View(await localDbContext.ToListAsync());
	}

	// GET: PaymentDetails/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var paymentDetail = await _context.PaymentDetails
			 .Include(p => p.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (paymentDetail == null) {
			return NotFound();
		}

		return View(paymentDetail);
	}

	// GET: PaymentDetails/Create
	public IActionResult Create() {
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName");
		return View();
	}

	// POST: PaymentDetails/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,CoffeeUserId,LastFour,ExpiryDate,GatewayToken,CreatedAt,DeletedAt")] PaymentDetail paymentDetail) {
		if (ModelState.IsValid) {
			_context.Add(paymentDetail);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", paymentDetail.CoffeeUserId);
		return View(paymentDetail);
	}

	// GET: PaymentDetails/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var paymentDetail = await _context.PaymentDetails.FindAsync(id);
		if (paymentDetail == null) {
			return NotFound();
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", paymentDetail.CoffeeUserId);
		return View(paymentDetail);
	}

	// POST: PaymentDetails/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,CoffeeUserId,LastFour,ExpiryDate,GatewayToken,CreatedAt,DeletedAt")] PaymentDetail paymentDetail) {
		if (id != paymentDetail.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(paymentDetail);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!PaymentDetailExists(paymentDetail.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", paymentDetail.CoffeeUserId);
		return View(paymentDetail);
	}

	// GET: PaymentDetails/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var paymentDetail = await _context.PaymentDetails
			 .Include(p => p.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (paymentDetail == null) {
			return NotFound();
		}

		return View(paymentDetail);
	}

	// POST: PaymentDetails/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var paymentDetail = await _context.PaymentDetails.FindAsync(id);
		if (paymentDetail != null) {
			_context.PaymentDetails.Remove(paymentDetail);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool PaymentDetailExists(long id) {
		return _context.PaymentDetails.Any(e => e.Id == id);
	}
}
