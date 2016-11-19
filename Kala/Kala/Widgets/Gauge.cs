﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace Kala
{
    public partial class Widgets
    {
        public static void Gauge(Grid grid, string header, JObject data)
        {
            int px = 0;
            int py = 0;
            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //If this fails, we dont know where to show an error
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                Debug.WriteLine("Label: " + widgetKeyValuePairs["label"]);

                px = Convert.ToInt16(widgetKeyValuePairs["px"]);
                py = Convert.ToInt16(widgetKeyValuePairs["py"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Gauge crashed: " + ex.ToString());
            }

            try
            {
                double start = Convert.ToDouble(widgetKeyValuePairs["min"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
                double end = Convert.ToDouble(widgetKeyValuePairs["max"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));

                AddHeaderText(grid, px, py, header);

                App.trackItem i = new App.trackItem
                {
                    grid = grid,
                    px = px,
                    py = py,
                    link = item.item.link,
                    min = widgetKeyValuePairs["min"],
                    max = widgetKeyValuePairs["max"],
                    state = item.item.state,
                    header = header,
                    unit = widgetKeyValuePairs["unit"],
                    icon = widgetKeyValuePairs["icon"],
                    type = "NumericItem"
                };
                App.config.items.Add(i);

                Gauge_update(true, grid, px, py, i.header, i.min, i.max, i.state, i.unit, i.icon, i.link);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Gauge crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }

        public static void Gauge_update(bool Create, Grid grid, int px, int py, string header, string s_min, string s_max, string s_state, string unit, string icon, string link)
        {
            try
            {
                double start = Convert.ToInt16(s_min);
                double end = Convert.ToInt16(s_max);

                double value = Convert.ToDouble(s_state.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
                if (start < 0)
                {
                    end = end + Math.Abs(start);
                    value = value + Math.Abs(start);
                    start = 0;
                }

                //Basic sanity checks
                if (value > end) end = value;
                if (value < start) start = value;

                int r = (int)((double)((value - start) / (double)(end - start) * 100.0));

                ProgressArc(grid, px, py, r, 0.7f);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Gauge crashed: " + ex.ToString());
            }

            AddCenterText(Create, grid, px, py, s_state, unit, link);
            AddImageEnd(grid, px, py, icon);
        }
    }
}