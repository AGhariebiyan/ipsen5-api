using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GMAPI.Models
{
    public class KnowledgeBase
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool Published { get; set; }

        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public bool Featured { get; set; }
    }
}
