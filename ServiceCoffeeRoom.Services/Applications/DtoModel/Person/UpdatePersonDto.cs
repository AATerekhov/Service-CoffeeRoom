using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceСoffeeRoom.Applications.DtoModel.Person
{
    public class UpdatePersonDto
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string TelegramAccaunt { get; set; }
    }
}
