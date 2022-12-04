﻿namespace angularapi.Models
{
    public class RefreshToken
    {
        public int ID { get; set; }
        public string Token { get; set; }
        public int UserID { get; set; }

        public RefreshToken(string token, int userID)
        {
            Token = token;
            UserID = userID;
        }

    }
}
