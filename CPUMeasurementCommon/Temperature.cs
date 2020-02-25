using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public class Temperature
    {
        private double? _value { get; set; }
        private MeasurementUnit _measurementUnit { get; set; }

        [JsonProperty("value")]
        public double? Value { get { return this._value; } set { this._value = value; } }

        [JsonProperty("measurementUnit")]
        public MeasurementUnit MeasurementUnit { get { return this._measurementUnit; } set { this._measurementUnit = value; } }

        public Temperature(double? value, MeasurementUnit measurementUnit)
        {
            this._value = value;
            this._measurementUnit = measurementUnit;
        }

        public Temperature()
        { 
            // Keep it for JSON de/serialization!
        }

        public Temperature InCelsius()
        {
            return this.convertToCelsius();
        }

        public Temperature InKelvin()
        {
            var temperature = this.convertToCelsius();
            var inKelvin = (this._value.HasValue ? temperature.Value + 273.15 : null);
            return new Temperature(inKelvin, MeasurementUnit.KELVIN);
        }

        public Temperature InFahrenheit()
        {
            var temperature = this.convertToCelsius();
            var inFahrenheit = (this._value.HasValue ? ((temperature.Value * 1.8) + 32) : null);
            return new Temperature(inFahrenheit, MeasurementUnit.FAHRENHEIT);
        }

        private Temperature convertToCelsius()
        {
            double? inCelsius = null;
            switch (this._measurementUnit)
            {
                case MeasurementUnit.KELVIN: inCelsius = (this._value.HasValue ? this._value - 273.15 : null); return new Temperature(inCelsius, MeasurementUnit.CELSIUS);
                case MeasurementUnit.FAHRENHEIT: inCelsius = (this._value.HasValue ? ((this._value - 32) / 1.8) : null); return new Temperature(inCelsius, MeasurementUnit.CELSIUS);
                default: return this;
            }
        }
    }
}
