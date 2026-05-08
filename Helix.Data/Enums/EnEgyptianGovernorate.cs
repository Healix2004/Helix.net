using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Data.Enums
{
    public enum EgyptianGovernorate
    {
        Cairo = 1,
        Alexandria = 2,

        [Display(Name = "Port Said")]
        [Description("Port Said")]
        PortSaid = 3,

        Suez = 4,
        Damietta = 11,
        Dakahlia = 12,
        Sharqia = 13,
        Qalyubia = 14,

        [Display(Name = "Kafr El Sheikh")]
        [Description("Kafr El Sheikh")]
        KafrElSheikh = 15,

        Gharbia = 16,
        Monufia = 17,
        Beheira = 18,
        Ismailia = 19,
        Giza = 21,

        [Display(Name = "Beni Suef")]
        [Description("Beni Suef")]
        BeniSuef = 22,

        Faiyum = 23,
        Minya = 24,
        Asyut = 25,
        Sohag = 26,
        Qena = 27,
        Aswan = 28,
        Luxor = 29,

        [Display(Name = "Red Sea")]
        [Description("Red Sea")]
        RedSea = 31,

        [Display(Name = "New Valley")]
        [Description("New Valley")]
        NewValley = 32,

        Matrouh = 33,

        [Display(Name = "North Sinai")]
        [Description("North Sinai")]
        NorthSinai = 34,

        [Display(Name = "South Sinai")]
        [Description("South Sinai")]
        SouthSinai = 35,

        [Display(Name = "Born Abroad")]
        [Description("Born Abroad")]
        BornAbroad = 88
    }
}
