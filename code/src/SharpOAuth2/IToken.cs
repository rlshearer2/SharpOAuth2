﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpOAuth2
{
    public interface IToken 
    {
        string Token { get; set; }
        string TokenType { get; set; }
        int ExpiresIn{ get; set; }
        string RefreshToken{ get; set; }
        long Created { get; }

        IDictionary<string, object> ToResponseValues();
    }
}
