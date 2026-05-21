namespace FileBox.Models.DBObjects;

public class DBTag
{
	public int TagId { get; set; }
	public string Name { get; set; }
	public int? ParentTagCode { get; set; }
	public string Colour { get; set; }
}
