﻿using System;

namespace QuantConnect.Indicators
{
    public class BollingerBands : Indicator
    {
        /// <summary>
        /// Gets the type of moving average
        /// </summary>
        public MovingAverageType MovingAverageType { get; private set; }

        /// <summary>
        /// Gets the standard deviation
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> StandardDeviation { get; private set; }

        /// <summary>
        /// Gets the middle bollinger band (moving average)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> MiddleBand { get; private set; }

        /// <summary>
        /// Gets the upper bollinger band (middleBand + k * stdDev)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> UpperBand { get; private set; }

        /// <summary>
        /// Gets the upper bollinger band (middleBand - k * stdDev)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> LowerBand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BollingerBands class
        /// </summary>
        /// <param name="period">The period of the standard deviation and moving average</param>
        /// <param name="k">The number of standard deviations specifying the distance of bands from the moving average</param>
        /// <param name="movingAverageType">The type of moving average to be used</param>
        public BollingerBands(int period, int k, MovingAverageType movingAverageType = MovingAverageType.Simple)
            : this(string.Format("BOL({0},{1})", period, k), period, k, movingAverageType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BollingerBands class
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the standard deviation and moving average</param>
        /// <param name="k">The number of standard deviations specifying the distance of the bands from the moving average</param>
        /// <param name="movingAverageType">The type of moving average to be used</param>
        public BollingerBands(String name, int period, int k, MovingAverageType movingAverageType = MovingAverageType.Simple)
            : base(name)
        {
            MovingAverageType = movingAverageType;
            StandardDeviation = new StandardDeviation(name + "_StandardDeviation", period);
            MiddleBand = movingAverageType.AsIndicator(name + "_MiddleBand", period);
            var kConstant = new ConstantIndicator<IndicatorDataPoint>(k.ToString(), (decimal)k);
            LowerBand = MiddleBand.Minus(StandardDeviation.Times(kConstant), name + "_LowerBand");
            UpperBand = MiddleBand.Plus(StandardDeviation.Times(kConstant), name + "_UpperBand");
        }
           
        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady
        {
            get { return MiddleBand.IsReady && UpperBand.IsReady && LowerBand.IsReady; }
        }

        /// <summary>
        /// Computes the next value of the sub-indicators from the given state:
        /// StandardDeviation, MiddleBand, UpperBand, LowerBand
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>The input is returned unmodified.</returns>
        protected override decimal ComputeNextValue(IndicatorDataPoint input)
        {
            StandardDeviation.Update(input);
            MiddleBand.Update(input);
            UpperBand.Update(input);
            LowerBand.Update(input);
            return input;
        }
    }
}
