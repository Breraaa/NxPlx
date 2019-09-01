using NxPlx.Models.Details;

namespace NxPlx.Models.Database
{
    public class ProducedBy
    {
        public int DetailsEntityId { get; set; }
        public DetailsEntityBase DetailsEntity { get; set; }
        
        public int ProductionCompanyId { get; set; }
        public ProductionCompany ProductionCompany { get; set; }
    }
}