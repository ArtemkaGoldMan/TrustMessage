using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty; // Will store Markdown content
        public string Signature { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
    }
}
