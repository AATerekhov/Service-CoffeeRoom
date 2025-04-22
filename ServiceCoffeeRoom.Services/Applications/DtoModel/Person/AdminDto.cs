using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCoffeeRoom.Services.Applications.DtoModel.Person
{
    public class AdminDto
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string TelegramAccaunt { get; set; }
    }
}
