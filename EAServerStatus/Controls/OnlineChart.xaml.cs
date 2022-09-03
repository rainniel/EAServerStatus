using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace EAServerStatus.Controls
{
    public partial class OnlineChart : UserControl
    {
        private const int MaxRecord = 60;
        private const double CorrectionRangePercentage = 0.3;

        private readonly List<int?> _onlineList = new List<int?>();
        private int _maxOnline;

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
            if (_onlineList.Count == 0) _onlineList.Add(online);

            _onlineList.Add(online);
            if (_onlineList.Count > MaxRecord)
            {
                _onlineList.RemoveAt(0);
            }

            CorrectData();

            var max = _onlineList.Max();
            _maxOnline = max == null ? 0 : (int)max;

            UpdateChart();
        }

        private void CorrectData()
        {
            if (_onlineList.Count >= 3)
            {
                bool tryToCorrect = false;
                var nullStartIndex = 0;

                for (var i = 0; i < _onlineList.Count; i++)
                {
                    if (tryToCorrect)
                    {
                        if (_onlineList[i] != null)
                        {
                            var lastValue = (int)_onlineList[nullStartIndex - 1];
                            var correctionOffset = (int)Math.Round((lastValue * CorrectionRangePercentage) / 2);

                            if (_onlineList[i] >= (lastValue - correctionOffset)
                           && _onlineList[i] <= (lastValue + correctionOffset))
                            {
                                var nullGaps = (i - nullStartIndex) + 1;
                                var difference = lastValue - (int)_onlineList[i];
                                var increment = Math.Abs(difference / nullGaps);
                                var correctionValue = lastValue;

                                if (_onlineList[i] >= lastValue)
                                {
                                    for (var i2 = nullStartIndex; i2 < i; i2++)
                                    {
                                        correctionValue += increment;
                                        _onlineList[i2] = correctionValue;
                                    }
                                }
                                else
                                {
                                    for (var i2 = nullStartIndex; i2 < i; i2++)
                                    {
                                        correctionValue -= increment;
                                        _onlineList[i2] = correctionValue;
                                    }
                                }
                            }

                            tryToCorrect = false;
                        }
                    }
                    else
                    {
                        if (i > 0 && i < _onlineList.Count)
                        {
                            if (_onlineList[i - 1] != null && _onlineList[i] == null)
                            {
                                tryToCorrect = true;
                                nullStartIndex = i;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateChart()
        {
            var plotModel = new PlotModel();
            var series = new AreaSeries
            {
                TrackerFormatString = "{4:###,###}"
            };

            int? previousOnline = null;
            var loop = 1;

            foreach (int? online in _onlineList)
            {
                var x = MaxRecord - (_onlineList.Count - loop);

                if (online == null)
                {
                    series.Points.Add(new DataPoint(x, 0));
                }
                else
                {
                    if ((loop == 1 && loop < _onlineList.Count && _onlineList[loop] != null)
                        || (loop > 1 && _onlineList[loop - 2] == null))
                    {
                        series.Points.Add(new DataPoint(x, 0));
                    }

                    if (loop == 1
                        || loop == _onlineList.Count
                        || previousOnline != online
                        || loop < _onlineList.Count && _onlineList[loop] == null
                        || loop < _onlineList.Count && _onlineList[loop] != online)
                    {
                        series.Points.Add(new DataPoint(x, (double)online));
                    }

                    if (loop < _onlineList.Count && _onlineList[loop] == null)
                    {
                        series.Points.Add(new DataPoint(x, 0));
                    }
                }

                previousOnline = online;
                loop++;
            }

            var stringFormat = _maxOnline < 1000 ? "###" : "#,K";

            var yMax = _maxOnline + (_maxOnline * 0.1);
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
