using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public class Temperature
    {
        private double _value { get; set; }
        private MeasurementUnit _measurementUnit { get; set; }

        public double Value { get { return this._value; } }
        public MeasurementUnit MeasurementUnit { get { return this._measurementUnit; } }

        public Temperature(double value, MeasurementUnit measurementUnit)
        {
            this._value = value;
            this._measurementUnit = MeasurementUnit;
        }

        public Temperature InCelsius()
        {
            return this.convertToCelsius();
        }

        public Temperature InKelvin()
        {
            var temperature = this.convertToCelsius();
            return new Temperature(temperature.Value+273.15, MeasurementUnit.KELVIN);
        }

        public Temperature InFahrenheit()
        {
            var temperature = this.convertToCelsius();
            return new Temperature(((temperature.Value*1.8) + 32), MeasurementUnit.KELVIN);
        }

        private Temperature convertToCelsius()
        {
            switch (this._measurementUnit)
            {
                case MeasurementUnit.KELVIN:  return new Temperature(this._value - 273.15, MeasurementUnit.CELSIUS);
                case MeasurementUnit.FAHRENHEIT: return new Temperature( ((this._value-32)/1800), MeasurementUnit.CELSIUS);
                default: return this;
            }
        }
    }
}
