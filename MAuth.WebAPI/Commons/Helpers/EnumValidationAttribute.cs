﻿using System.ComponentModel.DataAnnotations;

namespace MAuth.WebAPI.Commons.Validators;

public class EnumValueValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValueValidationAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum", nameof(enumType));
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 允许为空（选填），不进行验证
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        var input = value.ToString();
        var displayName = validationContext.DisplayName;

        if (Enum.TryParse(_enumType, input, ignoreCase: true, out var result)
            && Enum.IsDefined(_enumType, result)) return ValidationResult.Success;
        
        var validNames = string.Join("、", Enum.GetNames(_enumType));
        return new ValidationResult($"{displayName}的值必须为：{validNames}");

    }
}
