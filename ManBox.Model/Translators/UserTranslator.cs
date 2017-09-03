using ManBox.Model.ViewModels;

namespace ManBox.Model.Translators
{
    public static class UserTranslator
    {
        public static UserViewModel ToViewModel(this User user)
        {
            if (user == null)
                return null;

            return new UserViewModel()
            {
                Email = user.Email,
                Token = user.Token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.UserId
            };
        }
    }
}
