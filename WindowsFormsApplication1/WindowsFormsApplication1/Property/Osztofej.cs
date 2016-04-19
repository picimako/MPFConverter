﻿using System;

namespace MPFConverterApp
{
    class Osztofej
    {
        public bool Enabled { get; set; }
        public string Value { get; set; }

        public Osztofej()
        {
            Enabled = false;
            Value = String.Empty;
        }

        public Osztofej(bool enabled, String value)
        {
            Enabled = enabled;
            Value = value;
        }
    }
}
