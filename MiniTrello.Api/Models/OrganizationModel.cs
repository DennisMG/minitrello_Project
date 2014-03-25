﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class OrganizationModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
    }
}