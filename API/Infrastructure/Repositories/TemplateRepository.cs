using API.Core.Domain.Interfaces;

namespace API.Infrastructure.Repositories;

public class TemplateRepository : ITemplateRepository
{
    private readonly AppDbContext _db;

    public TemplateRepository(AppDbContext db)
    {
        _db = db;
    }
}
