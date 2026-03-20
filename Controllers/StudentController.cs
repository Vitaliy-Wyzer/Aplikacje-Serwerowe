using Microsoft.AspNetCore.Mvc;
using lab1_AS.Models;

namespace lab1_AS.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index(int id=1)
        {
            var Students = new List<Student>
            {
                new Student
                {
                    Id = 1,
                    Imie = "Imie1",
                    Nazwisko = "Nazwisko1",
                    NumerIndeksu = 130000,
                    DataUrodzenia = DateOnly.MaxValue,
                    Kierunek = "Informatyka"
                },
                new Student
                {
                    Id = 2,
                    Imie = "Imie2",
                    Nazwisko = "Nazwisko2",
                    NumerIndeksu = 140000,
                    DataUrodzenia = DateOnly.MaxValue,
                    Kierunek = "Logistyka"
                },
                new Student
                {
                    Id = 3,
                    Imie = "Imie3",
                    Nazwisko = "Nazwisko3",
                    NumerIndeksu = 150000,
                    DataUrodzenia = DateOnly.MaxValue,
                    Kierunek = "Budownictwo"
                }
            };

            return View(Students[id-1]);
        }
    }
}
