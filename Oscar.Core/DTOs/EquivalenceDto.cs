using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record EquivalenceDto
    {
        public string? CLIENT { get; set; }
        public string? TITLE { get; set; }
        public string? DECLARATION_ID { get; set; }
        public string? ISAN_WORK { get; set; }
        public string? AGICOA_NO { get; set; }
        public string? YOUR_REFERENCE { get; set; }

        public string? DECLARATION_STATUS { get; set; }
        public string? ISIN_STATUS { get; set; }
        public string? AGICOA_NO_STATUS { get; set; }
        public string? STATUS { get; set; }

        //Parameterless constructor required for serializer
        public EquivalenceDto() { }

    }

    public sealed class EquivalenceDtoMap : CsvHelper.Configuration.ClassMap<EquivalenceDto>
    {
        public EquivalenceDtoMap()
        {
            Map(m => m.CLIENT).Optional();
            Map(m => m.TITLE).Optional();
            Map(m => m.DECLARATION_ID);
            Map(m => m.ISAN_WORK);
            Map(m => m.AGICOA_NO);
            Map(m => m.YOUR_REFERENCE);
            Map(m => m.DECLARATION_STATUS).Optional();
            Map(m => m.ISIN_STATUS).Optional();
            Map(m => m.AGICOA_NO_STATUS).Optional();
            Map(m => m.STATUS).Optional();

        }
    }



}