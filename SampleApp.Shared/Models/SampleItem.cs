namespace SampleApp.Shared.Models
{
    public class SampleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

		public override string ToString()
		{
			return string.Format("[SampleItem: Id={0}, Title={1}, Description={2}]", Id, Title, Description);
		}
    }
}
