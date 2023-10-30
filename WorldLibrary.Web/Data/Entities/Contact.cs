using System.ComponentModel.DataAnnotations;

namespace WorldLibrary.Web.Data.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

   
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public Customer Customer { get; set; }

        public Employee Employee { get; set; }
    }
}
