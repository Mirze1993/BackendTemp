using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Domain.Tool;
using Ganss.Xss;


namespace Domain.ValidAttributes;

public class DecodeHtmlAttribute:ValidationAttribute
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
public class ClearHtmlAttribute:ValidationAttribute
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

public class NoHtmlAttribute:ValidationAttribute
{
    private static readonly Regex ScriptRegex = new Regex(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex HtmlTagRegex = new Regex(@"<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
   

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue && !string.IsNullOrEmpty(strValue))
        {
            if (HtmlTagRegex.IsMatch(strValue)||ScriptRegex.IsMatch(strValue))
            {
               throw new ValidationException(validationContext.MemberName +" is not valid");
            }
        }
       
        return ValidationResult.Success;
    
    }

}