using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    //public static class AquariumObjectFactory
    //{
    //    public static AquariumObject CreateByType(Type type, int X, int Y, Aquarium aquarium)
    //    {
    //        Type type = Type.GetType(typeof(AquariumObjects).Namespace + "." + obj.ToString(), throwOnError: false);

    //        if (type == null)
    //        {
    //            throw new InvalidOperationException(obj.ToString() + " is not a known Aquarium Object type");
    //        }

    //        return (AquariumObject)Activator.CreateInstance(type, aquarium, X, Y);
    //    }

    //    public static AquariumObject CreateByString(string readFrom)
    //    {
    //        return null;//todo
    //    }
    //    public static AquariumObject CreateByName(string name, Aquarium container, int X, int Y)
    //    {
    //        switch (name)
    //        {
    //            case "PredatorFish":
    //                {
    //                    return new PredatorFish(container, X, Y, (new Random()).Next(2) == 1, 0, 100);
    //                }
    //        }
    //    }
    //    public static AquariumObject CreateByNameParams(string name, dynamic parameters)
    //    {
    //        switch (name)
    //        {
    //            case "PredatorFish":
    //                {
    //                    return new PredatorFish(parameters.Aquarium, parameters.X, parameters.Y, parameters.Gender, parameters.Age, parameters.Satiety, parameters.PregnancyTime);
    //                }
    //            case "HerbivoreFish":
    //                {
    //                    return new HerbivoreFish(parameters.Aquarium, parameters.X, parameters.Y, parameters.Gender, parameters.Age, parameters.Satiety, parameters.PregnancyTime);
    //                }
    //        }
    //    }
    //}

}
