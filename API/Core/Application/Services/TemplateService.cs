using API.Core.Application.Interfaces;
using API.Core.Domain.Interfaces;

namespace API.Core.Application.Services;

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _templateRepository;

    public TemplateService(ITemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }
}
