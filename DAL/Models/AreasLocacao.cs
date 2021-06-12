﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class AreasLocacao : AbstractEntity
    {
        [Required]
        public string descricao { get; set; }

        [Required]
        public decimal valor { get; set; }
    }
}
