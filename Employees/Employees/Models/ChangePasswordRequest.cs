﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Models
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
