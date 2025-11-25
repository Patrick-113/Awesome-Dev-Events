namespace AwesomeDevEvents.API.Entities
{
  public class DevEvent
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<DevEventSpeaker> Speakers { get; set; }
    public bool IsDeleted { get; set; }

    public void Update(string title, string description, DateTime startdate, DateTime endDate)
    {
      Title = title;
      Description = description;
      StartDate = startdate;
      EndDate = endDate;
    }

    public void Delete()
    {
      IsDeleted = true;
    }
  }
}