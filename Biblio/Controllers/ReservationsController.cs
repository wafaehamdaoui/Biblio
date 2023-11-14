using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblio.Data;
using Biblio.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace Biblio.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        
        private readonly AppDbContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(AppDbContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
              return _context.Resevations != null ? 
                          View(await _context.Resevations.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Resevations'  is null.");
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Resevations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resevations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public int NumReservationsBySubscriber(int subscriberId)
        {
            var reservations = _context.Resevations
                .Where(r => r.SubscriberId == subscriberId)
                .Where(r => r.Status == false)
                .ToList();
            return reservations.Count;
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            List<Book> books = _context.Books.ToList();
            SelectList bookList = new SelectList(books.Where(obj=>obj.isAvailable==true), "Id", "title");
            ViewData["BookList"] = bookList;

            List<Subscriber> subscribers = _context.Subscribers.ToList();
            SelectList subscribersList = new SelectList(subscribers, "Id", "CardId");
            ViewData["SubscribersList"] = subscribersList;

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,BookId,TakeDate,BackDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (NumReservationsBySubscriber(reservation.SubscriberId) < 2)
                {
                    if (reservation.BackDate - reservation.TakeDate < TimeSpan.FromDays(14))
                    {
                        _context.Add(reservation);
                        var book = await _context.Books.FindAsync(reservation.BookId);
                        book.isAvailable = false;
                        _context.Update(book);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewData["ValidateMessage"] = "The duration of reservation must not exceed 2 weeks!";
                    }
                }
                else
                {
                    ViewData["ValidateMessage"] = "The subscriber could not get another book he already have two reservations!";
                }
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Resevations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resevations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,BookId,TakeDate,BackDate,Status")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Resevations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Resevations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Resevations == null)
            {
                return Problem("Entity set 'AppDbContext.Resevations'  is null.");
            }
            var reservation = await _context.Resevations.FindAsync(id);
            if (reservation != null)
            {
                _context.Resevations.Remove(reservation);
                var book = _context.Books.Find(reservation.BookId);
                book.isAvailable = true;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Resevations?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> BooksReservedBySubscriber(int subscriberId)
        {
            var reservations = await _context.Resevations
                .Where(r => r.SubscriberId == subscriberId)
                .OrderBy(r => r.Status)
                .ToListAsync();
            return View(reservations);
        }

        public IActionResult returnBook(int id)
        {
            
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var reservation = _context.Resevations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }
            reservation.Status = true;
            if (ModelState.IsValid)
            {
                _context.Resevations.Update(reservation);
                _context.SaveChanges();
                TempData["success"] = "reservation Updated successfuly!";
                return RedirectToAction("BooksReservedBySubscriber");
            }
            if (reservation.BookId == null || reservation.BookId == 0)
            {
                return NotFound();
            }
            var book = _context.Books.Find(reservation.BookId);
            if (book == null)
            {
                return NotFound();
            }
            book.isAvailable = true;
            if (ModelState.IsValid)
            {
                book.updatedAt = DateTime.Now;
                _context.Books.Update(book);
                _context.SaveChanges();
                TempData["success"] = "Book returned!";
                return RedirectToAction("BooksReservedBySubscriber");
            }

            return RedirectToAction("BooksReservedBySubscriber");

        }

    }
}
