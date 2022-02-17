using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarCenterGearFinder.Classes
{
    public class ListedItem
    {
        public double ItemNumber { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public decimal Price { get; set; }
        public string Link { get; set; }

        public ListedItem()
        {

        }

        public ListedItem(double itemNumber, string name, string condition, decimal price, string link)
        {
            this.ItemNumber = itemNumber;
            this.Name = name;
            this.Condition = condition;
            this.Price = price;
            this.Link = link;
        }

    }
}
