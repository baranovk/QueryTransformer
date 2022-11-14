using System.Text;
using Regex.Engine.Exceptions;
using Regex.Engine.Rules;

namespace Regex.Engine
{
    public class PatternParser
    {
        #region Fields

        private static readonly HashSet<char> _specialSymbols = new() { 's', 'r', 'n' };

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static Pattern Parse(string template, int? startIndex = null, int? endIndex = null)
        {
            // guard against empty template
            var i = startIndex.HasValue ? startIndex.Value - 1 : - 1;
            endIndex ??= template.Length - 1;

            var pattern = new Pattern();
            PatternRule? rule = null;
            Modificators? modificators = null;

            while (++i <= endIndex)
            {
                switch (template[i])
                {
                    case '^':
                        if(0 != i) throw new InvalidPatternException(i, template);
                        modificators = Modificators.FromStartOnly;
                        break;
                    case '$':
                        if (template.Length - 1 != i || null == rule) throw new InvalidPatternException(i, template);
                        modificators = Modificators.AtEndOnly;
                        SetModificators(rule, ref modificators);
                        return pattern;
                    case '\\':
                        rule = ParseSpecialSymbolRule(ref i, template);
                        break;
                    case '[':
                        rule = ParseSymbolGroup(ref i, template);
                        break;
                    case '.':
                        rule = ParseAnySymboRule(ref i, template);
                        break;
                    case '(':
                        rule = ParseGroupRule(ref i, template);
                        break;
                    default:
                        rule = ParseSingleSymbolRule(ref i, template);
                        break;
                }

                if (null != rule) 
                {
                    SetModificators(rule, ref modificators);
                    pattern.Add(rule); 
                }
            }

            return pattern;
        }

        #endregion

        #region Private Methods

        private static PatternRule? ParseSpecialSymbolRule(ref int position, string template)
        {
            if (!Read(++position, template, out char currentChar)) return null;

            return _specialSymbols.Contains(currentChar)
                ? ParseSymbolClassRule(ref position, template)
                : ParseSingleSymbolRule(ref position, template);
        }

        private static PatternRule? ParseSymbolClassRule(ref int position, string template)
        {
            if (!Read(position, template, out char currentChar)) return null;
            PatternRule? rule = null;

            switch (currentChar)
            {
                case 's':
                    rule = new SpaceRule();
                    rule.Quantity = ParseQuantity(ref position, template);
                    break;
                case 'n':
                    rule = new SingleSymbolRule('\n');
                    rule.Quantity = ParseQuantity(ref position, template);
                    break;
                case 'r':
                    rule = new SingleSymbolRule('\r');
                    rule.Quantity = ParseQuantity(ref position, template);
                    break;
            }

            return rule;
        }

        private static PatternRule? ParseSingleSymbolRule(ref int position, string template)
        {
            if (!Read(position, template, out char currentChar)) return null;
            var rule = new SingleSymbolRule(currentChar);
            rule.Quantity = ParseQuantity(ref position, template);
            return rule;
        }

        private static PatternRule? ParseAnySymboRule(ref int position, string template)
        {
            var rule = new AnySymbolRule();
            rule.Quantity = ParseQuantity(ref position, template);
            return rule;
        }

        private static PatternRule? ParseSymbolGroup(ref int position, string template)
        {
            var initPosition = position;
            var rule = new SymbolGroupRule();
            var state = SymbolGroupParsingState.None;
            char prevChar = default;
            var counter = 0;

            while (Read(++position, template, out var currentChar) && ']' != currentChar)
            {
                counter++;

                switch (currentChar)
                {
                    case '^':
                        if (1 < counter)
                        {
                            throw new InvalidPatternException(position, template);
                        }
                        rule.SetMode(SymbolGroupMode.Exclusive);
                        break;
                    case '-':
                        if (state == SymbolGroupParsingState.SingleSymbol)
                        {
                            state = SymbolGroupParsingState.Range;
                        }
                        else
                        {
                            throw new InvalidPatternException(position, template);
                        }
                        break;
                    case '\\':
                        state = SymbolGroupParsingState.SpecialSymbol;
                        break;
                    default:
                        if (SymbolGroupParsingState.SingleSymbol == state || SymbolGroupParsingState.None == state)
                        {
                            state = SymbolGroupParsingState.SingleSymbol;
                            rule.AddSymbol(currentChar);
                        }
                        else if (state == SymbolGroupParsingState.Range)
                        {
                            for (int i = prevChar; i <= currentChar; i++)
                            {
                                rule.AddSymbol((char)i);
                            }

                            state = SymbolGroupParsingState.None;
                        }
                        else if (state == SymbolGroupParsingState.SpecialSymbol)
                        {
                            if (_specialSymbols.Contains(currentChar))
                                throw new InvalidPatternException(position, template);

                            rule.AddSymbol(currentChar);
                            state = SymbolGroupParsingState.SingleSymbol;
                        }
                        prevChar = currentChar;
                        break;
                }
            }

            if (!(SymbolGroupParsingState.SingleSymbol == state || SymbolGroupParsingState.None == state) || 0 == counter)
                throw new InvalidPatternException(initPosition, template);

            rule.Quantity = ParseQuantity(ref position, template);
            return rule;
        }

        private static PatternRule? ParseGroupRule(ref int position, string template)
        {
            int? terminatorPosition = null;

            for (var i = position + 1; i < template.Length; i++)
            {
                if (')' == template[i] && '\\' != template[i - 1]) 
                {
                    terminatorPosition = i;
                    break;
                } 
            }

            if(!terminatorPosition.HasValue) throw new InvalidPatternException(position, template);
            if(1 == (terminatorPosition.Value - position)) throw new InvalidPatternException(position, template);
            
            var pattern = Parse(template, position + 1, terminatorPosition.Value - 1);
            var rule = new GroupRule(pattern);
            position = terminatorPosition.Value;
            rule.Quantity = ParseQuantity(ref position, template);
            return rule;
        }

        private static bool Read(int position, string input, out char c)
        {
            if (position < input.Length)
            {
                c = input[position];
                return true;
            }

            c = default;
            return false;
        }

        private static Quantificator ParseQuantity(ref int currentPosition, string template)
        {
            if (!Read(currentPosition + 1, template, out char currentChar)) return new Quantificator();

            switch (currentChar)
            {
                case '?':
                    currentPosition++;
                    return new Quantificator(0, 1);
                case '*':
                    currentPosition++;
                    return new Quantificator(0, null);
                case '+':
                    currentPosition++;
                    return new Quantificator(1, null);
                case '{':
                    currentPosition++;
                    var sb = new StringBuilder(10);
                    var state = QunatityParsingState.From;
                    var quantity = new Quantificator(null, null);
                    while (Read(++currentPosition, template, out currentChar) && '}' != currentChar)
                    {
                        if (currentChar.IsNumber())
                        {
                            sb.Append(currentChar);
                        }
                        else if (',' == currentChar && state == QunatityParsingState.From)
                        {
                            state = QunatityParsingState.To;
                            quantity.From = 0 < sb.Length ? Convert.ToInt32(sb.ToString()) : 1;
                            sb.Clear();
                        }
                        else
                        {
                            throw new InvalidPatternException(currentPosition, template);
                        }
                    }

                    if (sb.Length > 0 && state == QunatityParsingState.From)
                    {
                        quantity.From = quantity.To = Convert.ToInt32(sb.ToString());
                        sb.Clear();
                    }
                    else if (sb.Length > 0)
                    {
                        quantity.To = Convert.ToInt32(sb.ToString());
                        sb.Clear();
                    }

                    return quantity;
                default:
                    return new Quantificator();
            }
        }

        private static void SetModificators(PatternRule rule, ref Modificators? modificators)
        {
            rule.Modificators = modificators;
            modificators = null;
        }

        #endregion

        #region Utility

        private enum QunatityParsingState
        {
            From,
            To
        }

        private enum SymbolGroupParsingState
        {
            None,
            SingleSymbol,
            SpecialSymbol,
            Range
        }

        #endregion
    }
}
