﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    
    static class Temp
    {
        private  static string temp { get; set; }

        static public void Add(char c)
        {
            temp += c;
        }
        static public string Empty()
        {
            string value = temp;
            temp = string.Empty;
           
            return value;
        }

        static public string GetValue() 
        {
            return temp;
        }

        static public int Length() 
        {
            return temp.Length;
        }
       
    }
}
