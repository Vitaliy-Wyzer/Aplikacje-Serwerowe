namespace lab1_AS.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int NumerIndeksu { get; set; }
        public DateOnly DataUrodzenia { get; set; }
        public string Kierunek {  get; set; }

    }
}
