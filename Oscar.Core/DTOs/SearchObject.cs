using System;
namespace Oscar.Core.DTOs
{
    public record SearchObject(string? SearchEntity, string? SearchColumnType, string? SearchColumn, string? SearchText);
}

