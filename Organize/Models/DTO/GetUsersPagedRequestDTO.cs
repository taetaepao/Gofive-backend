namespace Organize.Models.DTO
{
    public class GetUsersPagedRequestDTO
    {
        public int Totalcount { get; set; }
        public IEnumerable<GetUsersRequestDTO> Users { get; set; } = new List<GetUsersRequestDTO>();
    }
}
