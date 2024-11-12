using System;
namespace NGO.Model
{
    public class RefreshTokenRequestModel
    {
        public int UserId { get; set; }
        public Guid RefreshToken { get; set; }
    }

}

