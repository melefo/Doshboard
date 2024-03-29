﻿using System.ComponentModel.DataAnnotations;

namespace Doshboard.Backend.Models.Widgets
{
    public class GameModel
    {
        [Required]
        public string Id { get; set; }
        public string? Name { get; set; }
    }
}
