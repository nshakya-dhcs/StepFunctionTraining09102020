using System;
using System.Collections.Generic;

namespace Demo.Lib.Data
{
    public class JobInfo
    {
        public string JobId { get; set; }
        public bool Resolved { get; set; }
        public int Data { get; set; }

        public string[] ECSPayload { get; set; }
        public bool Override { get; set; }
    }

    public class JobWithTaskToken
    {
        public string TaskToken { get; set; }
        public JobInfo State { get; set; }
    }

    public class Input
    {
        public int Value { get; set; }
        public string Result { get; set; }
        public bool Override { get; set; }
    }
}
