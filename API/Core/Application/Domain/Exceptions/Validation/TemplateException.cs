using API.Core.Domain.Exceptions;

namespace API.Core.Domain.Exceptions.Validation;

public class TemplateException : AppException
{
    public TemplateException()
        : base("Template exception occurred.", "TEMPLATE_EXCEPTION", 400) { }

    public TemplateException(string message)
        : base(message, "TEMPLATE_EXCEPTION", 400) { }
}
