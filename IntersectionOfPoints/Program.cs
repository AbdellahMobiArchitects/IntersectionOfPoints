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
using System.Diagnostics;

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
            var (listUser1, listUser2) = GetFakeData(100);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            ProcessList(listUser1, listUser2);

            sw.Stop();
            Console.WriteLine($"Elapsed={sw.Elapsed.Humanize()}");
            Console.ReadKey();
        }

        static (List<UserGeoData> listOne, List<UserGeoData> listTwo) GetFakeData(int numberOfLines)
        {
            var dataFaker = new Faker<UserGeoData>()
                .StrictMode(true)
                .RuleFor(o => o.Latitude, f => f.Random.Double(33.600000, 33.600600))
                .RuleFor(o => o.Longitude, f => f.Random.Double(-7.600000, -7.600000))
                .RuleFor(o => o.DateReported, f => f.Date.Between(DateTime.Parse("01/01/2019 10:00"), DateTime.Parse("01/01/2019 11:00")));

            return (dataFaker.Generate(numberOfLines), dataFaker.Generate(numberOfLines));
        }

        static void ProcessList(List<UserGeoData> list1, List<UserGeoData> list2)
        {
            foreach (var datai in list1)
            {
                var posi = new Position(new Longitude(datai.Longitude), new Latitude(datai.Latitude));
                foreach (var dataj in list2)
                {
                    // get position
                    var posj = new Position(new Longitude(dataj.Longitude), new Latitude(dataj.Latitude));

                    // get distance
                    var distance = posi.DistanceTo(posj);

                    // check distance
                    if (!(distance <= Distance.FromMeters(10)))
                        // continue if the desired distance isn't met
                        continue;

                    // get interval
                    var interval = datai.DateReported.Subtract(dataj.DateReported);

                    // check interval
                    if (interval <= TimeSpan.FromMinutes(10))
                        Console.WriteLine($">>>>>> ORANGE PERSON <<<<<<");
                }
            }
        }
    }
}
