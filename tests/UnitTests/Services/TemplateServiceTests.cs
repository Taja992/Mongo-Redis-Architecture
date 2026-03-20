using API.Core.Application.Services;
using API.Core.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace UnitTests.Services;

public class TemplateServiceTests
{
    private readonly ITemplateRepository _repository = Substitute.For<ITemplateRepository>();
    private readonly TemplateService _sut;

    public TemplateServiceTests()
    {
        _sut = new TemplateService(_repository);
    }

    // TODO: add tests here
}
