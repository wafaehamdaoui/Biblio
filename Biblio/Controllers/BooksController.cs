using Biblio.Data;
using Biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblio.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _db;
        public BooksController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Book> bookList = _db.Books;

            return View(bookList);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                book.createdAt = DateTime.Now;
                book.updatedAt = DateTime.Now;
                book.isAvailable = true;
                _db.Books.Add(book);
                _db.SaveChanges();
                TempData["success"] = "Book added successfully!";
                return RedirectToAction("Index");
            }
            return View(book);
        }
        // Get: 
        public  IActionResult Details(int id)
        {
            if (id == null || _db.Books == null)
            {
                return NotFound();
            }

            var book =  _db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        //Get
        public IActionResult Edit(int? id)
        {
            if (id==null || id==0)
            {
                return NotFound();
            }
            var book = _db.Books.Find(id);
            if(book == null)
            {
                return NotFound();
            }
            return View(book);
        }


        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                book.updatedAt = DateTime.Now;
                _db.Books.Update(book);
                _db.SaveChanges();
                TempData["success"] = "Book updated successfully!";
                return RedirectToAction("Index");
            }
            return View(book);
        }

        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var book = _db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Book book)
        {
            if (book!=null)
            {
                _db.Books.Remove(book);
                _db.SaveChanges();
                TempData["success"] = "Book deleted successfully!";
                return RedirectToAction("Index");
            }
            return View(book);
        }
    }

}
