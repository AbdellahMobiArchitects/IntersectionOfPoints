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
using System.Threading;

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
        private static object obj = new object();

        const int MINIMUM_DISTANCE_METERS = 10;
        const int MINIMUM_INTERVAL_MINUTES = 10;

        static int loopCount = 0;
        static int matchCount = 0;

        static void Main(string[] args)
        {
            var (listUser1, listUser2) = GetFakeData(100);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            ProcessList(
                sw,
                listUser1,
                listUser2,
                MINIMUM_INTERVAL_MINUTES,
                MINIMUM_DISTANCE_METERS);

            sw.Stop();

            Console.WriteLine($"total loops => {loopCount}");
            Console.WriteLine($"total match => {matchCount}");
            Console.WriteLine($"elapsed time => {sw.Elapsed.Humanize()} // {sw.Elapsed.TotalMilliseconds}");
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

        static void ProcessList(
            Stopwatch sw,
            List<UserGeoData> list1,
            List<UserGeoData> list2,
            int intervalInMinutes,
            int distanceInMeters)
        {
            int maximum = list1.Count * list2.Count;

            Parallel.ForEach(list1, (datai) =>
            {
                var posi = new Position(new Longitude(datai.Longitude), new Latitude(datai.Latitude));

                foreach (var dataj in list2)
                {
                    // get position
                    var posj = new Position(new Longitude(dataj.Longitude), new Latitude(dataj.Latitude));

                    // get distance
                    var distance = posi.DistanceTo(posj);

                    //var distance2 = DistanceMetresSEP(datai.Latitude, datai.Longitude, dataj.Latitude, dataj.Longitude);


                    // check distance
                    if (distance <= Distance.FromMeters(distanceInMeters))
                    //if (distance2 <= Distance.FromMeters(distanceInMeters).Value)
                    {
                        // get interval
                        var interval = datai.DateReported.Subtract(dataj.DateReported);

                        // check interval
                        if (interval <= TimeSpan.FromMinutes(intervalInMinutes))
                        {
                            Interlocked.Increment(ref matchCount);
                        }
                    }

                    Interlocked.Increment(ref loopCount);
                }
            });
        }

        #region Helpers
        #region private: const
        private const double _radiusEarthMiles = 3959;
        private const double _radiusEarthKM = 6371;
        private const double _m2km = 1.60934;
        private const double _toRad = Math.PI / 180;
        #endregion
        public static double DistanceMetresSEP(double Lat1,
                                      double Lon1,
                                      double Lat2,
                                      double Lon2)
        {
            try
            {
                double _radLat1 = Lat1 * _toRad;
                double _radLat2 = Lat2 * _toRad;
                double _dLat = (_radLat2 - _radLat1);
                double _dLon = (Lon2 - Lon1) * _toRad;

                double _a = (_dLon) * Math.Cos((_radLat1 + _radLat2) / 2);

                // central angle, aka arc segment angular distance
                double _centralAngle = Math.Sqrt(_a * _a + _dLat * _dLat);

                // great-circle (orthodromic) distance on Earth between 2 points
                return _radiusEarthMiles * _centralAngle * 1609;
            }
            catch { throw; }
        }
        #endregion
    }
}
