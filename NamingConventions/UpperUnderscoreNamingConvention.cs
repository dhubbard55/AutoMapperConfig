using System.Text.RegularExpressions;
using AutoMapper;

namespace AutoMapperConfig.NamingConventions
{
    public class UpperUnderscoreNamingConvention : INamingConvention
    {
        private readonly Regex _splittingExpression = new Regex("[\\p{Lu}0-9]+(?=_?)");

        public Regex SplittingExpression
        {
            get
            {
                return _splittingExpression;
            }
        }

        public string SeparatorCharacter
        {
            get
            {
                return "_";
            }
        }
    }
}
