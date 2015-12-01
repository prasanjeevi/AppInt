using Restinfinity.Utility.HelpPage.ModelDescriptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Restinfinity.Net.Models
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public static IList<Parameter> GetParameters(Collection<ParameterDescription> param)
        {
            List<Parameter> d = new List<Parameter>();
            foreach (var p in param)
            {
                d.Add(new Parameter() { Name = p.Name, Type = p.TypeDescription.Name });
            }
            return d;
        }

        public static IList<Parameter> GetParameters(ModelDescription modelDescription)
        {
            ComplexTypeModelDescription complexTypeModelDescription = modelDescription as ComplexTypeModelDescription;
            if (complexTypeModelDescription != null)
            {
                return Parameter.GetParameters(complexTypeModelDescription.Properties);
            }

            CollectionModelDescription collectionModelDescription = modelDescription as CollectionModelDescription;
            if (collectionModelDescription != null)
            {
                complexTypeModelDescription = collectionModelDescription.ElementDescription as ComplexTypeModelDescription;
                if (complexTypeModelDescription != null)
                {
                    return Parameter.GetParameters(complexTypeModelDescription.Properties);
                }
            }

            return new List<Parameter>();
        }
    }
}