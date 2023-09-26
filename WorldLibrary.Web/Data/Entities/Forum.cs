namespace WorldLibrary.Web.Data.Entities
{
    public class Forum : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Menssage { get; set; }
        public int Assessment { get; set; }
    }
}
