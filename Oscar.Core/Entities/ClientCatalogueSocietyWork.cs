namespace Oscar.Core.Entities
{
    public class ClientCatalogueSocietyWork
    {
        public int WorksId { get; init; }
        public int ParentWorksid { get; init; }
        public string Discriminator { get; init; }
        public int CatalogueId { get; init; }
    }
}