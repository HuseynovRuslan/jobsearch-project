namespace Boss.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public string Location { get; set; }
    public decimal Salary { get; set; }
    public int EmployerId { get; set; }
    public string Email { get; set; }
}
