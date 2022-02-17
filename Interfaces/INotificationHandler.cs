using GuitarCenterGearFinder.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarCenterGearFinder.Interfaces
{
    public interface INotificationHandler
    {
        INotificationData PrepareData(ListedItem listedItem);
        void SendMessage(INotificationData message, string destinationEmail, ListedItem listedItem);

        void MarkItemAsSeen(ListedItem listedItem);
    }
}
