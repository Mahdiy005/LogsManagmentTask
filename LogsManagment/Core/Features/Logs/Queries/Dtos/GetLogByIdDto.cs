namespace LogsManagment.Core.Features.Logs.Queries.Dtos
{
    public class GetLogByIdDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<UserForLogDto> Users { get; set; }
    }
}
