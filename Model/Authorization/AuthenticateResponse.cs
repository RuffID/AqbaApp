﻿using System;

namespace AqbaApp.Model.Authorization
{
    public class AuthenticateResponse()
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
