using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeEcommerce.Application.Features.Users.DTOs
{
    public class BlockUserRequest
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int? ExpiresInDays { get; set; }
    }
}
