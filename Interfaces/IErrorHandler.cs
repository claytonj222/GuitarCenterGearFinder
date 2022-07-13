using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarCenterGearFinder.Interfaces
{
    public interface IErrorHandler
    {
        void SendError(Exception ex, string destination);
        INotificationData PrepareData(Exception ex);
    }
}
