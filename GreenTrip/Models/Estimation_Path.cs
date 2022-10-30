using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GreenTrip.Models.Estimatation;

namespace GreenTrip.Models
{
    public class Estimation_Path
    {
        public List <estimatation> estimatations { get; set; }
        public List <Location> locations { get; set; }
    }
}