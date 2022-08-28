using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace EAServerStatus.Controls
{
    public partial class OnlineChart : UserControl
    {
        private const int MaxRecord = 60;

        private readonly List<int?> onlineList = new List<int?>();
        private int maxOnline;

        public OnlineChart()
        {
            InitializeComponent();

            var plotModel = new PlotModel();
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 1,
                AbsoluteMaximum = 10,
                IsZoomEnabled = false
            });

            PvwChart.Model = plotModel;
        }

        public void AddOnline(int? online)
        {
            onlineList.Add(online);
            if (onlineList.Count > MaxRecord)
            {
                onlineList.RemoveAt(0);
            }

            var max = onlineList.Max();
            maxOnline = max == null ? 0 : (int)max;

            UpdateChart();
        }

        private void UpdateChart()
        {
            var plotModel = new PlotModel();
            var series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                TrackerFormatString = "{4:###,###}"
            };

            int? previousOnline = null;
            int loop = 1;

            foreach (int? online in onlineList)
            {
                if (online == null)
                {
                    series.Points.Add(new DataPoint(MaxRecord - (onlineList.Count - loop), double.NaN));
                }
                else
                {
                    if (loop == 1
                        || loop == onlineList.Count
                        || previousOnline != online
                        || loop < onlineList.Count && onlineList[loop] == null)
                    {
                        series.Points.Add(new DataPoint(MaxRecord - (onlineList.Count - loop), (double)online));
                    }
                }

                previousOnline = online;
                loop++;
            }

            var stringFormat = "###";

            if (maxOnline > 999)
            {
                stringFormat = "#,K";
            }

            var yMax = maxOnline + (maxOnline * 0.1);
            if (yMax < 10) yMax = 10;

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                StringFormat = stringFormat,
                Minimum = 1,
                Maximum = yMax,
                AbsoluteMinimum = 1,
                AbsoluteMaximum = yMax,
                IsZoomEnabled = false
            };

            var xAxis = new DateTimeAxis
            {
                Minimum = 1,
                Maximum = MaxRecord,
                AbsoluteMinimum = 1,
                AbsoluteMaximum = MaxRecord,
                MinimumRange = 1,
                IsZoomEnabled = false,
                IsAxisVisible = false
            };

            plotModel.Series.Add(series);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            PvwChart.Model = plotModel;
        }
    }
}
