using Helix.Data.Enums;
using System;

namespace Helix.Service.Helper
{
    public class EgyptainNationalId
    {
        public EgyptianGovernorate governorate;
        public DateOnly dateOfBirth;
        public EnGenders gender;
        public string NationalId { get; private set; }

        public EgyptainNationalId(string nationalId)
        {
            this.NationalId = nationalId;
            NationalId = nationalId.Trim();
            if (string.IsNullOrWhiteSpace(NationalId) || NationalId.Length != 14 || !long.TryParse(NationalId, out _))
            {
                throw new ArgumentException("Invalid National ID. It must consist of exactly 14 digits.");
            }
            try
            {
                // 1. Extract Date of Birth
                int centuryCode = int.Parse(nationalId.Substring(0, 1)); // 1st digit
                int year = int.Parse(nationalId.Substring(1, 2));        // 2nd and 3rd digits
                int month = int.Parse(nationalId.Substring(3, 2));       // 4th and 5th digits
                int day = int.Parse(nationalId.Substring(5, 2));         // 6th and 7th digits

                // Calculate the birth year based on the century code (2 for 1900s, 3 for 2000s)
                int fullYear = (centuryCode == 2 ? 1900 : centuryCode == 3 ? 2000 : 0) + year;

                this.dateOfBirth = new DateOnly(fullYear, month, day);

                // 2. Extract Governorate of Birth
                string govCode = nationalId.Substring(7, 2);             // 8th and 9th digits
                if (!int.TryParse(govCode, out int govNum))
                {
                    throw new ArgumentException("Invalid governorate code in National ID.");
                }

                // Map numeric code to EgyptianGovernorate enum if defined; otherwise use BornAbroad as fallback
                if (Enum.IsDefined(typeof(EgyptianGovernorate), govNum))
                {
                    this.governorate = (EgyptianGovernorate)govNum;
                }
                else
                {
                    // Some IDs use 88 for born abroad; choose BornAbroad or throw depending on your rules
                    this.governorate = EgyptianGovernorate.BornAbroad;
                }

                // 3. Extract Gender
                int genderCode = int.Parse(nationalId.Substring(12, 1)); // 13th digit
                this.gender = (genderCode % 2 != 0) ? EnGenders.Male : EnGenders.Female; // Odd is Male, Even is Female 
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred while parsing the National ID. The date or other parts might be invalid.", ex);
            }
        }
    }

    public static class StringExtensions
    {
        public static EgyptainNationalId ParseEgyptianId(this string nationalId)
        {
            return new EgyptainNationalId(nationalId);
        }
    }
}