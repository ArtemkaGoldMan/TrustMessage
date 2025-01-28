using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    public class CreateMessageDTO
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters.")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
