using PhoneNumbers;

namespace ShoeEcommerce.Application.Common.Helpers;
public static class PhoneNumberHelper
{
    private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();
    public static string DefaultCountry { get; set; } = "IN";
    public static bool IsValid(string phoneNumber, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode ?? DefaultCountry);

            return PhoneUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
    public static string? Normalize(string phoneNumber, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode ?? DefaultCountry);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
                return null;

            return PhoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    public static string? FormatInternational(string phoneNumber, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode ?? DefaultCountry);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
                return null;

            return PhoneUtil.Format(parsedNumber, PhoneNumberFormat.INTERNATIONAL);
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    public static string? FormatNational(string phoneNumber, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode ?? DefaultCountry);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
                return null;

            return PhoneUtil.Format(parsedNumber, PhoneNumberFormat.NATIONAL);
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    public static int? GetCountryCode(string phoneNumber, string? defaultCountryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, defaultCountryCode ?? DefaultCountry);
            return parsedNumber.CountryCode;
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    public static string? GetRegionCode(string phoneNumber, string? defaultCountryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, defaultCountryCode ?? DefaultCountry);
            return PhoneUtil.GetRegionCodeForNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    /// Checks if phone number is mobile (vs landline)
    /// NOTE: Not 100% accurate for all countries
    /// Some countries don't distinguish mobile from landline in numbering
 
    public static bool? IsMobile(string phoneNumber, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode ?? DefaultCountry);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
                return null;

            var numberType = PhoneUtil.GetNumberType(parsedNumber);

            return numberType == PhoneNumberType.MOBILE ||
                   numberType == PhoneNumberType.FIXED_LINE_OR_MOBILE;
        }
        catch (NumberParseException)
        {
            return null;
        }
    }
}