using NewsAggregator.Entity;

namespace NewsAggregator.Dto
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
    }
}
