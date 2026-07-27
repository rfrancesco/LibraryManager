namespace LibraryManager
{
    public class LoanQueryDto : BaseQueryDto
    {
        public int? BookId { get; set; }
        public int? UserId { get; set; }
        public bool? Active { get; set; }
    }
}