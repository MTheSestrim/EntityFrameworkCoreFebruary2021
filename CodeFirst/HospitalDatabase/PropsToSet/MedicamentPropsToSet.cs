namespace P01_HospitalDatabase.PropsToSet
{
    class MedicamentPropsToSet : ISettableProperties
    {
        public MedicamentPropsToSet()
        {
            PropertiesToSet = new string[] { "Name" };
        }
        public string[] PropertiesToSet { get; }
    }
}
