using DotSpatial.Data;
using DotSpatial.Positioning;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

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

            var listUser1 = new List<UserGeoData>()
            {
                new UserGeoData 
                { 
                    Latitude=33.588225,
                    Longitude=-7.683082,
                    DateReported=DateTime.Parse("17/04/2019 23:00"), 
                },
                new UserGeoData
                {
                    Latitude=33.588520,
                    Longitude=-7.682417,
                    DateReported=DateTime.Parse("17/04/2019 23:00"),
                },
                new UserGeoData 
                {
                    Latitude=33.589512,
                    Longitude=-7.680336,
                    DateReported=DateTime.Parse("17/04/2019 23:00"),
                },
            };

            var listUser2 = new List<UserGeoData>()
            {
                new UserGeoData
                {
                    Latitude=33.589494,
                    Longitude=-7.680379,
                    DateReported=DateTime.Parse("17/04/2019 22:00"),
                },
                new UserGeoData
                {
                    Latitude=33.589494,
                    Longitude=-7.680379,
                    DateReported=DateTime.Parse("17/04/2019 23:00"),
                },
                new UserGeoData
                {
                    Latitude=33.589494,
                    Longitude=-7.680379,
                    DateReported=DateTime.Parse("17/04/2019 23:30"),
                },
            };

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
