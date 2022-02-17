using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarCenterGearFinder.Interfaces
{
    public interface INotificationData
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
