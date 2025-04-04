namespace VectorSearchDemos.Core.Entities;

public class Shoe
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public float[] DescriptionVector { get; set; }
  public string ImageUrl { get; set; }
  public float[] ImageVector { get; set; }
}
