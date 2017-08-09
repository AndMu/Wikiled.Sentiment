using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Reflect a sociological perspective, especially as reflected in the writings of Talcott Parsons. 
    /// A high score reflects use of the language of that institution, talking like a lawyer, professor, or military officer
    /// </summary>
    public class InstitutionData : DataItem
    {
        /// <summary>
        /// A clear political character, including political roles, collectivities, acts, ideas, ideologies, and symbols.
        /// </summary>
        [InfoField("Polit@")]
        public bool IsPolitical { get; private set; }

        /// <summary>
        /// Relating to academic, intellectual or educational matters, including the names of major fields of study
        /// </summary>
        [InfoField("Academ")]
        public bool IsAcademic { get; private set; }

        /// <summary>
        /// Referring to organized systems of belief or knowledge, including those of applied knowledge, mystical beliefs, and arts that academics study.
        /// </summary>
        [InfoField("Doctrin")]
        public bool IsDoctrine { get; private set; }

        /// <summary>
        /// Economic, commercial, industrial, or business orientation, including roles, collectivities, acts, abstract ideas, and symbols, 
        /// including references to money. Includes names of common commodities in business.
        /// </summary>
        [InfoField("Econ@")]
        public bool IsEconomic { get; private set; }

        /// <summary>
        /// Concerned with buying, selling and trading.
        /// </summary>
        [InfoField("Exch")]
        public bool IsExchange { get; private set; }

        /// <summary>
        /// Associated with the arts, sports, and self-expression.
        /// </summary>
        [InfoField("Exprsv")]
        public bool IsExpression { get; private set; }

        /// <summary>
        /// Relating to legal, judicial, or police matters.
        /// </summary>
        [InfoField("Legal")]
        public bool IsLegal { get; private set; }

        /// <summary>
        /// Relating to military matters.
        /// </summary>
        [InfoField("Milit")]
        public bool IsMilitary { get; private set; }

        /// <summary>
        /// Pertaining to religious, metaphysical, supernatural or relevant philosophical matters.
        /// </summary>
        [InfoField("Relig")]
        public bool IsReligious { get; private set; }

        public override string Name => "Institution";
    }
}
