using System;
using System.Collections.Generic;
using System.Text;

namespace ServerProDiscord
{
    public class Argument
    {
        public delegate void InvokeSig(string value);

        public string[] name;
        public InvokeSig invoke;
        public string description;
        public bool required;
        public bool supplied;
        public Argument(string[] n, InvokeSig i, string d, bool r)
        {
            name = n;
            invoke = i;
            description = d;
            required = r;
            supplied = false;
        }
    }
}
