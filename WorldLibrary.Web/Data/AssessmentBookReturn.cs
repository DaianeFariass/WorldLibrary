using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Data
{
    public class AssessmentBookReturn : IEntity
    {
        public int Id { get; set; }

        public int Assessment { get; set; }
    }
}
