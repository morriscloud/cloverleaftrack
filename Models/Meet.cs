﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloverleafTrack.Models
{
    public class Meet
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public Guid SeasonId { get; set; }
        [NotMapped] public string UrlName => "";

        public Season Season { get; set; }
        public MeetResult MeetResult { get; set; }
        public List<School> Schools { get; set; } = new();
        public List<Performance> Performances { get; set; } = new();
    }
}
