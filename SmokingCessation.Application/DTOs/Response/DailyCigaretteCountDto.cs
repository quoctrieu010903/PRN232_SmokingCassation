using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class DailyCigaretteCountDto
    {
        public DateTime Date { get; set; }
        public int CigarettesSmoked { get; set; }
    }
}
