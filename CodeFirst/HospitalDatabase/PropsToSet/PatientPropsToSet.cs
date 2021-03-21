namespace P01_HospitalDatabase.PropsToSet
{
    class PatientPropsToSet : ISettableProperties
    {
        public PatientPropsToSet()
        {
            PropertiesToSet = new string[] { "FirstName", "LastName", "Address", "Email", "HasInsurance" };
        }
        public string[] PropertiesToSet { get; }
    }
}
