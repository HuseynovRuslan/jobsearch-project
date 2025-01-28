namespace Boss.Models;

public class Worker:User
{
  
    public List<CV> CVs { get; set; } = new();
}
