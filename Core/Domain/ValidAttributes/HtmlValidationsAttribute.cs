using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Domain.Tool;
using Ganss.Xss;


namespace Domain.ValidAttributes;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DecodeHtmlFromClassAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        EncodeHtmlRecursively(value);

        return ValidationResult.Success;
    }

    private void EncodeHtmlRecursively(object obj)
    {
        if (obj == null) return;

        var type = obj.GetType();
        if (type == typeof(string)) return;

        // Əgər bu kolleksiyadırsa (List, Array və s.)
        if (obj is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                EncodeHtmlRecursively(item);
            return;
        }

        // Public property-ləri tap
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite) continue;

            var propType = prop.PropertyType;
            var propValue = prop.GetValue(obj);

            if (propValue == null) 
                continue;


            if (propValue is string str && !string.IsNullOrEmpty(str))
            {
                var clean = CustomHtmlSanitizer.Sanitize(str);
                prop.SetValue(obj, clean);
            }
            else if (!propType.IsPrimitive && propType != typeof(decimal) && propType != typeof(DateTime))
            {
                EncodeHtmlRecursively(propValue);
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DecodeHtmlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            var clean = CustomHtmlSanitizer.Sanitize(str);
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (property != null && property.CanWrite)
                property.SetValue(validationContext.ObjectInstance, clean);
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// html taglarini icersi ile birlikde silir
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ClearHtmlWitValueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            var sanitized = new HtmlSanitizer();
            sanitized.AllowedTags.Clear();
            sanitized.AllowedSchemes.Clear();
            var clean = sanitized.Sanitize(str);
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (property != null && property.CanWrite)
                property.SetValue(validationContext.ObjectInstance, clean);
        }

        return ValidationResult.Success;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NoHtmlAttribute : ValidationAttribute
{
    private static readonly Regex ScriptRegex =
        new Regex(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex HtmlTagRegex = new Regex(@"<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);


    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue && !string.IsNullOrEmpty(strValue))
        {
            if (HtmlTagRegex.IsMatch(strValue) || ScriptRegex.IsMatch(strValue))
            {
                throw new ValidationException(validationContext.MemberName + " is not valid");
            }
        }

        return ValidationResult.Success;
    }
}