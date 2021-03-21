namespace P01_HospitalDatabase.PropsToSet
{
    class PatientMedicamentPropsToSet : ISettableProperties
    {
        public PatientMedicamentPropsToSet()
        {
            PropertiesToSet = new string[] { "PatientId", "MedicamentId" };
        }
        public string[] PropertiesToSet { get; }
    }
}
