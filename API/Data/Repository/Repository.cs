namespace API.Data;

public class Repository(DataContext context) : IRepository
{
    private DataContext _context = context;
}