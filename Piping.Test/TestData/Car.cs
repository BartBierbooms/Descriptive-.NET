using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class Car : IExposeLog
    {

        public const decimal CARPRICE = 18000M;
        public const string ToyotaMark = "Toyota";
        public const string HondaMark = "Honda";

        public string Mark { get; set; }
        public int Speed { get; set; }
        public bool NeedFuel { get; set; }
        public bool Parked { get; set; }

        public string Log => $" Cars is of mark: {Mark}, Car has a speed of: {Speed}, Car need Fuel {NeedFuel}, Car is parked: {Parked}";
        public const int SpeedFast = 50;
        public const int SpeedSlow = 10;
        public const int NoSpeed = 0;
        public decimal Price { get; set; }

        public Car()
        {
            Price = CARPRICE;
        }

        public void SetMark(string mark) {
            Mark = mark;
        }

        public Car SetParked(bool isParked)
        {
            Parked = isParked;
            return this;
        }

        public Car SetParked(bool isParked, Storage state)
        {
            state.AddToState(this);
            Parked = isParked;
            return this;
        }
    }

    public static class CarExt
    {

        public static Car SetOnStorage(this Car source, Storage state)
        {
            state.AddToState(source);
            return source;
        }
        public static Car AddCarToState(this Car source, IValueAndSupplement<Storage, Car> input)
        {
            input.Val.AddToState(input.SupplementVal);
            return source;
        }

        public static Car Validate(this Car source)
        {
            if (source.Mark == Car.HondaMark || source.Mark == Car.ToyotaMark)
            {
                return source;
            }
            throw new Exception("buy a decant car");
        }

        public static Car DriveFast(this Car source)
        {
            source.Speed = Car.SpeedFast;
            return source;
        }

        public static CarWithIInvariant DriveFast(this CarWithIInvariant source)
        {
            ((Car)source).DriveFast();
            return source;
        }

        public static Car DriveSlow(this Car source)
        {
            source.Speed = Car.SpeedSlow;
            return source;
        }

        public static CarWithIInvariant DriveSlow(this CarWithIInvariant source)
        {
            ((Car)source).DriveSlow();
            return source;
        }

        public static Car DriveFar(this Car source)
        {
            source.NeedFuel = true;
            return source;
        }

        public static CarWithIInvariant DriveFar(this CarWithIInvariant source)
        {
            ((Car)source).NeedFuel = true;
            return source;
        }

        public static Car Park(this Car source)
        {
            source.Parked = true;
            return source;
        }

        public static CarWithIInvariant Park(this CarWithIInvariant source)
        {
            ((Car)source).Parked = true;
            return source;
        }


        public static bool IsVW(this Car source)
        {
            return source.Mark == "VW";
        }

        public static bool IsHonda(this Car source)
        {
            return source.Mark == Car.HondaMark;
        }
        public static bool IsToyota(this Car source)
        {
            return source.Mark == Car.ToyotaMark;
        }
    }
    }
