using Microsoft.AspNetCore.Mvc;
using System.Text;
using UserNotepad.Models;

namespace UserNotepad.Controllers
{
    public class UserController : Controller
    {
        readonly NotepadDbContext _context;
        public UserController(NotepadDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Users);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Edit(int? id)
        {
            if (id==null)
                return NotFound();

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Birthday,Surname,Gender")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (user.Birthday>DateTime.UtcNow)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            else
            {
                return BadRequest();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (user.Birthday > DateTime.UtcNow)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        public IActionResult EditAttribute(int id,string attribname,string attribvalue="")
        {
            return View(new UserAttribute() { UserId=id, Name=attribname,Value=attribvalue});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttribute(int? id, UserAttribute attribute)
        {
            if (id == null)
                return NotFound();

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            if (!user.Attributes.ContainsKey(attribute.Name))
            {
                user.Attributes.Add(attribute.Name, attribute.Value);
            }

            user.Attributes[attribute.Name] = attribute.Value;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAttribute(int? id, string attribname)
        {
            if (id == null)
                return NotFound();

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            user.Attributes.Remove(attribname);
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GenerateReport()
        {
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms, Encoding.Unicode, -1, true))
            {
                sw.WriteLine($"Imię;Nazwisko;Data urodzenia;Płeć;Tytuł;Wiek");

                foreach (var item in _context.Users)
                {
                    sw.WriteLine($"{item.Name};{item.Surname};{item.Birthday.ToShortDateString()};{(item.Gender == Gender.Male ? "Mężczyzna" : "Kobieta")};{(item.Gender == Gender.Male ? "Pan" : "Pani")};{(int)((DateTime.Now - item.Birthday).TotalDays / 365f)}");
                }
            }
            ms.Position = 0;
            var fileName = $"report {DateTime.UtcNow.ToString("G")}.csv";
            return File(ms, "text/csv", fileName);
        }
    }
}
