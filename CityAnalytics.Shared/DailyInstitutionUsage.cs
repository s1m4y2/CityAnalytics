using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityAnalytics.Shared
{
    public class DailyInstitutionUsage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Institution { get; set; } = "";
        public int FullFare { get; set; }
        public int Student { get; set; }
        public int Teacher { get; set; }
        public int SixtyYearsOld { get; set; }
        public int Ticket { get; set; }
        public int Child { get; set; }
        public int Personnel { get; set; }
        public int Free { get; set; }
        public int BankCard { get; set; }
        public int Total => FullFare + Student + Teacher + SixtyYearsOld + Ticket + Child + Personnel + Free + BankCard;
    }
}

