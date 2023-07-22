namespace Trilha.DotNet.Shared.Extensions;

public static class ValidationExtensions
{
    public static IEnumerable<string> Validate<T, TAbstractValidator>(this T obj) where TAbstractValidator
        : AbstractValidator<T> where T : class
    {
        var validator = Activator.CreateInstance(typeof(TAbstractValidator)) as TAbstractValidator;

        var validationResult = validator?.Validate(obj);
        var isValid = validationResult?.IsValid ?? true;

        return (isValid ? Array.Empty<string>() :
            validationResult?.Errors.Select(s => s.ErrorMessage)) ?? Array.Empty<string>();
    }

    public static bool IsCnpj(this string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        cnpj = cnpj.Trim();

        cnpj = cnpj.Replace(".", string.Empty)
                   .Replace("-", string.Empty)
                   .Replace("/", "");

        if (cnpj.Length != 14)
            return false;

        var tempCnpj = cnpj[..12];
        var soma = 0;

        for (var i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        var resto = (soma % 11);

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        var digito = resto.ToString();
        tempCnpj += digito;

        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito += resto.ToString();
        return cnpj.EndsWith(digito);
    }

    public static bool IsCpf(this string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        cpf = cpf.Trim();

        cpf = cpf.Replace(".", string.Empty)
                 .Replace("-", string.Empty);

        if (cpf.Length != 11)
            return false;

        var tempCpf = cpf[..9];
        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (var i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito += resto.ToString();
        return cpf.EndsWith(digito);
    }

    public static bool IsEmail(this string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}