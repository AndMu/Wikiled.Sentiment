using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// References to places, locations and routes between them
    /// </summary>
    public class LocationData : DataItem
    {
        /// <summary>
        /// Place
        /// </summary>
        [InfoField("PLACE")]
        public bool IsPlace { get; private set; }

        /// <summary>
        /// Locations that typically provide for social interaction and occupy limited space
        /// </summary>
        [InfoField("Social")]
        public bool IsSocial { get; private set; }

        /// <summary>
        /// Region
        /// </summary>
        [InfoField("Region")]
        public bool IsRegion { get; private set; }

        /// <summary>
        /// Route
        /// </summary>
        [InfoField("Route")]
        public bool IsRoute { get; private set; }

        /// <summary>
        /// Aquatic
        /// </summary>
        [InfoField("Aquatic")]
        public bool IsAquatic { get; private set; }

        /// <summary>
        /// Places occurring in nature, such as desert or beach
        /// </summary>
        [InfoField("Land")]
        public bool IsLand { get; private set; }

        /// <summary>
        /// Any aerial conditions, natural vapors and objects in outer space)
        /// </summary>
        [InfoField("Sky")]
        public bool IsSky { get; private set; }

        public override string Name => "Location";
    }
}
