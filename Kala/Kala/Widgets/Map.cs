﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static List<Map> itemMaps = new List<Map>();

        public static void Map(Grid grid, string x1, string y1, string x2, string y2, string maptype, JArray data)
        {
            CrossLogger.Current.Debug("Map", "Creating Map Widget");

            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            //If this fails, we don't know where to show the error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Map", "Crashed: " + ex.ToString());
            }

            try
            {
                var map = new Map() //MapSpan.FromCenterAndRadius(new Position(37, -122), Distance.FromMiles(0.3)))
                {
                    IsShowingUser = true,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    MapType = GetMapType(maptype)
                };
                itemMaps.Add(map);

                //Items in Map widget
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
                CrossLogger.Current.Debug("Map", "Items: " + items.Count.ToString());

                var latitudes = new List<double>();
                var longitudes = new List<double>();

                foreach (Models.Sitemap.Widget3 item in items)
                {
                    //Add the Pins to list and map
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                    double lat = 0.0;
                    double lon = 0.0;
                    string url = String.Empty;
                    
                    if (item.item != null)
                    {
                        var b = item.item.state.Split(',');

                        lat = Convert.ToDouble(b[0]);
                        lon = Convert.ToDouble(b[1]);
                        url = item.item.link;
                    }
                    else
                    {
                        lat = Convert.ToDouble(widgetKeyValuePairs["lat"]);
                        lon = Convert.ToDouble(widgetKeyValuePairs["lon"]);
                    }

                    latitudes.Add(lat);
                    longitudes.Add(lon);

                    var pin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = widgetKeyValuePairs["label"],
                        Tag = url,
                        Position = new Position(lat, lon),
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Red)
                    };

                    //Color keywords
                    if (widgetKeyValuePairs.ContainsKey("color"))
                    {
                        pin.Icon = BitmapDescriptorFactory.DefaultMarker(GetColor(widgetKeyValuePairs["color"]));
                    }

                    map.Pins.Add(pin);
                }
                MapUpdate(latitudes, longitudes, map);

                grid.Children.Add(map, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Info("Map", "Crashed:" + ex.ToString());
            }
        }

        public static void MapUpdate(List<double>latitudes, List<double> longitudes, Map map)
        {
            //Show all pins
            //https://forums.xamarin.com/discussion/64984/forms-map-how-to-center-and-define-the-zoom-level-to-see-all-pins-and-the-user-position
            double lowestLat = latitudes.Min();
            double highestLat = latitudes.Max();
            double lowestLong = longitudes.Min();
            double highestLong = longitudes.Max();
            double finalLat = (lowestLat + highestLat) / 2;
            double finalLong = (lowestLong + highestLong) / 2;
            double distance = DistanceCalculation.GeoCodeCalc.CalcDistance(lowestLat, lowestLong, highestLat, highestLong, DistanceCalculation.GeoCodeCalcMeasurement.Kilometers);
            if (distance < 1) distance = 1;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(finalLat, finalLong), Distance.FromKilometers(distance)));
        }

        public static MapType GetMapType(string maptype)
        {
            switch (maptype.ToUpper())
            {
                case "NONE": return MapType.None;
                case "HYBRID": return MapType.Hybrid;
                case "SATELLITE": return MapType.Satellite;
                case "STREET": return MapType.Street;
                default: return MapType.Street;
            }            
        }

        public static Color GetColor(string color)
        {
            switch (color.ToLower())
            {
                case "aqua": return Color.Aqua;
                case "black": return Color.Black;
                case "blue": return Color.Blue;
                case "fuchsia": return Color.Fuchsia;
                case "gray": return Color.Gray;
                case "green": return Color.Green;
                case "lime": return Color.Lime;
                case "maroon": return Color.Maroon;
                case "navy": return Color.Navy;
                case "olive": return Color.Olive;
                case "orange": return Color.Orange;
                case "pink": return Color.Pink;
                case "purple": return Color.Purple;
                case "red": return Color.Red;
                case "silver": return Color.Silver;
                case "teal": return Color.Teal;
                case "white": return Color.White;
                case "yellow": return Color.Yellow;
                default: return Color.Red;
            }
        }
    }

    public class DistanceCalculation
    {
        public static class GeoCodeCalc
        {
            public const double EarthRadiusInMiles = 3956.0;
            public const double EarthRadiusInKilometers = 6367.0;

            public static double ToRadian(double val) { return val * (Math.PI / 180); }
            public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
            {
                return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
            }

            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
            {
                double radius = GeoCodeCalc.EarthRadiusInMiles;

                if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
                return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
            }
        }

        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }
    }
}
