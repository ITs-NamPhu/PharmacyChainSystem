using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System;

namespace PharmacyManagement.Models
{
    public class RefreshToken
    {
        public long RefreshTokenID { get; set; }

        public long UserID { get; set; }
        [StringLength(255)]
        public string Token { get; set; }

        public DateTime ExpireAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRevoked { get; set; }

        public User? User { get; set; }
    }
}
