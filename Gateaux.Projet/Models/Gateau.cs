using System;
using Gateaux.Projet.Models.Enums;

namespace Gateaux.Projet.Models
{
    internal class Gateau
    {
        internal StatusGateauEnum Status { get; set; }
   
        public Gateau()
        {
             Status = StatusGateauEnum.preparation;
         }

 
    }
}
