using Microsoft.AspNetCore.Mvc;
using LibraryMVC.Models;
using LibraryMVC.Data;

namespace LibraryMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _repo;

        public BookController(IBookRepository repo)
        {
            _repo = repo;
        }

        // GET: Book/Index — список книг
        public async Task<IActionResult> Index()
        {
            var books = await _repo.GetAllBooksAsync();

            // Если база вернула null, передаем пустой список
            if (books == null)
            {
                books = new List<LibraryMVC.Models.Book>();
            }

            return View(books);
        }

        // GET: Book/Details/5 — карточка книги
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var book = await _repo.GetBookByIdAsync(id.Value);
            if (book == null) return NotFound();
            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View(new Book());
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookID,Title,Author,Year,Genre,Pages,ISBN,SchemaXml")] Book book)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // POST: Book/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookID,Title,Author,Year,Genre,Pages,ISBN,SchemaXml")] Book book)
        {
            // ✅ Проверка: id должен совпадать с BookID в модели
            if (id != book.BookID)
            {
                return NotFound();
            }

            // ✅ Проверка ModelState - выведите ошибки если есть
            if (!ModelState.IsValid)
            {
                // Выводим все ошибки для отладки
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }

                return View(book);
            }

            try
            {
                // ✅ Вызываем репозиторий
                var result = await _repo.UpdateBookAsync(book);

                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Если репозиторий вернул false
                    TempData["Error"] = "Не удалось обновить книгу";
                    return View(book);
                }
            }
            catch (Exception ex)
            {
                // ✅ Ловим исключения для отладки
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return View(book);
            }
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _repo.GetBookByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            // ✅ Передаем книгу в представление
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var book = await _repo.GetBookByIdAsync(id.Value);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // API метод для получения XML схемы
        [HttpGet("GetSchemaXml/{id}")]
        public async Task<IActionResult> GetSchemaXml(int id)
        {
            var book = await _repo.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            return Content(book.SchemaXml ?? "", "text/xml");
        }
    }
}
