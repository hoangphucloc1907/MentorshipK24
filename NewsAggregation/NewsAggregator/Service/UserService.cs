using NewsAggregator.Entity;
using NewsAggregator.Repository;

namespace NewsAggregator.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public UserService(UserRepository userRepository, PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public void InsertUser(User user)
        {
            user.Password = _passwordHasher.HashPassword(user.Password);
            _userRepository.InsertUser(user);
        }

        public User? AuthenticateUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user != null && _passwordHasher.VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        public int? AuthenticateAndGetUserId(string email, string password)
        {
            var user = AuthenticateUser(email, password);
            return user?.Id;
        }
    }
}
