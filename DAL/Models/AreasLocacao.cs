﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class AreasLocacao : AbstractEntity
    {   
        public string descricao { get; set; }
        
        public decimal valor { get; set; }
    }
}
