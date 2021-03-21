namespace P01_HospitalDatabase.PropsToSet
{
    class VisitationPropsToSet : ISettableProperties
    {
        public VisitationPropsToSet()
        {
            PropertiesToSet = new string[] { "Date", "Comments", "PatientId" };
        }
        public string[] PropertiesToSet { get; }
    }
}
