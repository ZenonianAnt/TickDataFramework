using System;
using cAlgo.API;
using cAlgo.API.Internals;
using System.Collections.Generic;

//this class is a loader for tick data; instanciate and object by passing to the constructor
//the path to the desired file, start date, end date and data density expressed in 1 in X ticks
//once the loader has loaded tick data, it is possible to aggregate it in candles, to do this, use the
//TryLoadBarData method. The getters for the OHLCV data return lists, so make sure to store them
//in another list in you main class otherwise it would be heavy duty to call them frequently

namespace cAlgo
{
    public class LData
    {
        private List<double> Open;
        private List<double> High;
        private List<double> Low;
        private List<double> Close;
        private List<int> Volume;
        private List<Tick> Ticks;

        #region getters
        public List<double> GetOpen()
        {
            return this.Open;
        }

        public List<double> GetHigh()
        {
            return this.High;
        }

        public List<double> GetLow()
        {
            return this.Low;
        }

        public List<double> GetClose()
        {
            return this.Close;
        }

        public List<int> GetVolume()
        {
            return this.Volume;
        }

        public List<Tick> GetTicks()
        {
            return this.Ticks;
        }
        #endregion

        #region members
        public string Path { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Density { get; set; }
        #endregion

        public LData(string path, DateTime startDate, DateTime endDate, int Density)
        {
            this.Path = path;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Density = Density;
            Clean();
            CleanTicks();
        }

        public LData(string path, DateTime startDate, DateTime endDate, int Density, bool autoLoad)
        {
            this.Path = path;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Density = Density;
            Clean();
            CleanTicks();
            if (autoLoad)
                TryLoadTickData();
        }

        #region loaders
        public bool TryLoadTickData()
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(Path);

                for (int i = 1; i < lines.Length; i += Density)
                {
                    string[] currentTick = lines[i].Split(';');
                    if (currentTick.Length != 3) continue;
                    Tick tickToAdd = new Tick { Date = DateTime.Parse(currentTick[0]), Bid = double.Parse(currentTick[1]), Ask = double.Parse(currentTick[2]) };
                    if (tickToAdd.Date >= StartDate && tickToAdd.Date <= EndDate)
                        Ticks.Add(tickToAdd);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool TryLoadBarData(TimeSeries OpenTimes)
        {
            //this mathod aggregates data by scrolling through the Ticks list
            try
            {
                int tickIndex = 0;
                for (int i = 0; i <= OpenTimes.Count; ++i)
                {
                    int _Volume = 0;
                    double _Open = Ticks[tickIndex].Bid;
                    double _High = 0;
                    double _Low = double.PositiveInfinity;
                    double _Close = 0;
                    while (OpenTimes[i] > Ticks[tickIndex].Date && tickIndex < Ticks.Count)
                    {
                        ++_Volume;
                        _High = Math.Max(_High, Ticks[tickIndex].Bid);
                        _Low = Math.Min(_Low, Ticks[tickIndex].Bid);
                        _Close = Ticks[tickIndex].Bid;
                        ++tickIndex;
                    }
                    this.Open.Add(_Open);
                    this.High.Add(_High);
                    this.Low.Add(_Low);
                    this.Close.Add(_Close);
                    this.Volume.Add(_Volume);
                }
                //check for candle coherence to chart
                if (OpenTimes.Count != (Open.Count + High.Count + Low.Count + Close.Count + Volume.Count) / 4)
                    throw (new Exception("Incoherent number of candles, check data"));
            }
            catch (Exception) { return false; }
            return true;
        }
        #endregion

        public void Clean()
        {
            this.Open = new List<double>();
            this.High = new List<double>();
            this.Low = new List<double>();
            this.Close = new List<double>();
            this.Volume = new List<int>();
        }
        public void CleanTicks()
        {
            Ticks = new List<Tick>();
        }
    }

    public struct Tick
    {
        //the Tick data structure
        public DateTime Date { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
    }

}