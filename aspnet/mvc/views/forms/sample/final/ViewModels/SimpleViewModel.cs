﻿using System.ComponentModel.DataAnnotations;

namespace FormsTagHelper.ViewModels
{
    public class SimpleViewModel
    {
        [MaxLength(5000), MinLength(10), Required]
        public string Description { get; set; }
    }
}
