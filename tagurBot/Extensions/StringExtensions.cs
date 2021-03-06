﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tagurBot
{
    public static class StringExtensions
    {
        public static string ToFirstCharUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}