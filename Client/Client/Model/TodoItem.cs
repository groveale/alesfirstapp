﻿using Client.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class TodoItem : TableData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }

        public bool Photo { get; set; }
    }
}
