using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record ScreenrightsDto
    {
        public string? ClaimID { get; set; }
        public string? Your_Reference { get; set; }
        
        //Parameterless constructor required for serializer
        public ScreenrightsDto() { }

    }

    public sealed class ScreenrightsDtoMap : CsvHelper.Configuration.ClassMap<ScreenrightsDto>
    {
        public ScreenrightsDtoMap()
        {
            Map(m => m.ClaimID);
            Map(m => m.Your_Reference).Name("YourReference");
        }
    }



}