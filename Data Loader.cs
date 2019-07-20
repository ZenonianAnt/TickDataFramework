using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using System.Collections.Generic;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class DataLoader : Indicator
    {
        [Parameter("DB Path", DefaultValue = "")] //path to DB
        public string Path { get; set; }
        [Parameter("Data Density (1 in X)", DefaultValue = 1)]//data density, the program will take 1 in X points of data to reduce load
        public int Den { get; set; }

        protected override void Initialize()
        {
            LData LD = new LData(Path, MarketSeries.OpenTime[0], MarketSeries.OpenTime[Chart.BarsTotal], Den);
            Open = LD.GetOpen();
            High = LD.GetHigh();
            Low = LD.GetLow();
            Close = LD.GetClose();
            Volume = LD.GetVolume();
            Ticks = LD.GetTicks();
            if (Ticks[0].Date > MarketSeries.OpenTime[0])
                Chart.DrawStaticText("Error", "Error: Not Enough Tick Data for Selected Chart", VerticalAlignment.Top, HorizontalAlignment.Center, Color.AliceBlue);
        }

        public override void Calculate(int index)
        {
            if (IsLastBar)
            Ticks.Add(new Tick { Date = Time, Bid = Symbol.Bid, Ask = Symbol.Ask });
        }

        private List<double> Open;
        private List<double> High;
        private List<double> Low;
        private List<double> Close;
        private List<int> Volume;
        private List<Tick> Ticks;

        #region getters
        public double GetOpen(int i)
        {
            try
            {
                return this.Open[i];
            }
            catch (Exception) { return 0; }
        }

        public double GetHigh(int i)
        {
            try
            {
                return this.High[i];
            }
            catch (Exception) { return 0; }
        }

        public double GetLow(int i)
        {
            try
            {
                return this.Low[i];
            }
            catch (Exception) { return 0; }
        }

        public double GetClose(int i)
        {
            try
            {
                return this.Close[i];
            }
            catch (Exception) { return 0; }
        }

        public int GetVolume(int i)
        {
            try
            {
                return this.Volume[i];
            }
            catch (Exception) { return 0; }
        }

        public Tick GetTick(int i)
        {
            try
            {
                return this.Ticks[i];
            }
            catch (Exception) { return new Tick { Date = DateTime.MinValue, Bid = 0, Ask = 0 }; }
        }
        #endregion
    }
}
