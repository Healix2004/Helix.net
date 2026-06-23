namespace Helix.Data.Enums
{
    // Note: In real-world FHIR, 'Reaction' (Manifestation) is usually an open SNOMED search, 
    // but using an Enum with the most common reactions is perfect and much easier for your UI!
    public enum EnAllergyReaction
    {
        Hives,
        Rash,
        Nausea,
        Vomiting,
        ShortnessOfBreath,
        Swelling,
        Anaphylaxis,
        CutaneousHypersensitivity,
        NasalDischarge,
        Other,
        Unknown
    }
}
