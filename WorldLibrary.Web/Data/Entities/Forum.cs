using System;
using System.ComponentModel.DataAnnotations;

namespace WorldLibrary.Web.Data.Entities
{
    public class Forum : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Menssage { get; set; }
        public int Assessment { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }
    }
}
