using System.Net.NetworkInformation;

namespace WaktuSolatMY.Application.Constants
{
    public static class Zones
    {
        public static IReadOnlyList<string> Lists()
        {
            return new List<string>
            {
                "JHR01",
                "JHR02",
                "JHR03",
                "JHR04",
                "KDH01",
                "KDH02",
                "KDH03",
                "KDH04",
                "KDH05",
                "KDH06",
                "KDH07",
                "KTN01",
                "KTN02",
                "MLK01",
                "NGS01",
                "NGS02",
                "NGS03",
                "PHG01",
                "PHG02",
                "PHG03",
                "PHG04",
                "PHG05",
                "PHG06",
                "PLS01",
                "PNG01",
                "PRK01",
                "PRK01",
                "PRK02",
                "PRK03",
                "PRK04",
                "PRK05",
                "PRK06",
                "PRK07",
                "SBH01",
                "SBH02",
                "SBH03",
                "SBH04",
                "SBH05",
                "SBH06",
                "SBH07",
                "SBH08",
                "SBH09",
                "SWK01",
                "SWK02",
                "SWK03",
                "SWK04",
                "SWK05",
                "SWK06",
                "SWK07",
                "SWK08",
                "SWK09",
                "SGR01",
                "SGR02",
                "SGR03",
                "TRG01",
                "TRG02",
                "TRG03",
                "WLY01",
                "WLY02",
            };
        }

        public static class ClientName
        {
            public const string JAKIM = "JAKIM";
        }

        public static class BaseUrls
        {
            public const string JAKIM = "https://www.e-solat.gov.my/index.php";
        }

        public static class Date_FormatSupported
        {
            public static string[] Lists()
            {
                return new string[] {
                "dd-MMM-yyyy",
                "dd-MM-yyyy"
                };
            }
        }
    }
}
