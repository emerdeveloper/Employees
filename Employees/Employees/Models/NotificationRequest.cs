using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Models
{
    public class NotificationRequest
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
    }
}
