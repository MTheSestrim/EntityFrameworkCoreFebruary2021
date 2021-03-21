namespace P01_HospitalDatabase.PropsToSet
{
    class DiagnosePropsToSet : ISettableProperties
    {

        public DiagnosePropsToSet()
        {
            PropertiesToSet = new string[] { "Name", "Comments", "PatientId" };
        }
        public string[] PropertiesToSet { get; }
    }
}
