using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnapForm.Shared
{
    public struct InputTypes
    {
        public const string Button = "button";
        public const string Checkbox = "checkbox";
        public const string Color = "color";
        public const string Date = "date";
        public const string Email = "email";
        public const string File = "file";
        public const string Hidden = "hidden";
        public const string Image = "image";
        public const string Month = "month";
        public const string Number = "number";
        public const string Password = "password";
        public const string Radio = "radio";
        public const string Range = "range";
        public const string Reset = "reset";
        public const string Search = "search";
        public const string Tel = "tel";
        public const string Text = "text";
        public const string Time = "time";
        public const string Url = "url";
        public const string Week = "week";
        public const string Select = "select";
        public const string Location = "location";
        public const string Drawing = "drawing";

        public static readonly List<string> Types = new List<string>
        {
            Button,
            Checkbox,
            Color,
            Date,
            Email,
            File,
            Hidden,
            Image,
            Month,
            Number,
            Password,
            Radio,
            Range,
            Reset,
            Search,
            Tel,
            Text,
            Time,
            Url,
            Week,
            Select,
            Location,
            Drawing
        };

        public static Type GetType(string type)
        {
            return type switch
            {
                Button => typeof(string),
                Checkbox => typeof(bool),
                Color => typeof(Color),
                Date => typeof(DateTime),
                Email => typeof(string),
                File => typeof(byte[]),
                Hidden => typeof(string),
                Image => typeof(byte[]),
                Month => typeof(string),
                Number => typeof(long),
                Password => typeof(string),
                Radio => typeof(string[]),
                Range => typeof(int),
                Reset => typeof(string),
                Search => typeof(string),
                Tel => typeof(string),
                Text => typeof(string),
                Time => typeof(DateTime),
                Url => typeof(string),
                Week => typeof(string),
                Select => typeof(string),
                Location => typeof(SnapFormLocationEntry),
                _ => typeof(string)
            };
        }
    }

    public struct SearchTypes
    {
        public const string Occupation = "Occupation";

        public static readonly List<string> Types = new List<string>
        {
            Occupation
        };
    }
}