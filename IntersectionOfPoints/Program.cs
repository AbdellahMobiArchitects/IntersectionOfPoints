using DotSpatial.Data;
using DotSpatial.Positioning;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Bogus;

namespace IntersectionOfPoints
{
    class Program
    {
        public class UserGeoData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public DateTime DateReported { get; set; }
        }

        static void Main(string[] args)
        {

            var dataFaker = new Faker<UserGeoData>()
                .StrictMode(true)
                .RuleFor(o => o.Latitude, f => f.Random.Double(33.600000, 33.600600))
                .RuleFor(o => o.Longitude, f => f.Random.Double(-7.600000, -7.600000))
                .RuleFor(o => o.DateReported, f => f.Date.Between(DateTime.Parse("01/01/2019 10:00"), DateTime.Parse("01/01/2019 11:00")));

            var listUser1 = dataFaker.Generate(100);
            var listUser2 = dataFaker.Generate(100);

            foreach (var datai in listUser1)
            {
                var posi = new Position(new Longitude(datai.Longitude), new Latitude(datai.Latitude));

                foreach (var dataj in listUser2)
                {
                    var posj = new Position(new Longitude(dataj.Longitude), new Latitude(dataj.Latitude));
                    var interval = datai.DateReported.Subtract(dataj.DateReported);
                    var intervalHumanized = interval.Humanize();
                    var distance = posi.DistanceTo(posj);

                    Console.WriteLine($"Interval: {intervalHumanized}");
                    Console.WriteLine($"distance: {distance}");

                    if(distance <= Distance.FromMeters(10)
                        && interval <= TimeSpan.FromMinutes(10))
                        Console.WriteLine($">>>>>> ORANGE PERSON <<<<<<");

                    Console.WriteLine($"******");
                }
            }

            Console.ReadKey();
        }
    }
}
