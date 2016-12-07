﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tagurBot
{
    public class LuisResponse
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public object[] entities { get; set; }
        public Dialog dialog { get; set; }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
        public Action[] actions { get; set; }
    }

    public class Action
    {
        public bool triggered { get; set; }
        public string name { get; set; }
        public Parameter[] parameters { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public bool required { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string entity { get; set; }
        public string type { get; set; }
        public Resolution resolution { get; set; }
    }

    public class Resolution
    {
    }

    public class Dialog
    {
        public string contextId { get; set; }
        public string status { get; set; }
    }

}