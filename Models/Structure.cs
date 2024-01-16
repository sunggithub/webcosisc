namespace Website.Models
{
    public class Structure
    {
        public int StructureId { get; set; }
        public string StructureName { get; set; }

        public Structure()
        {
            if (StructureName == null)
            {
                StructureName = "";
            }

        }
    }
}
