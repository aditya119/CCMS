namespace CCMS.Shared.Models.AppUserModels
{
    public class UserDetailsModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserFullname { get; set; }
        public int UserRoles { get; set; }
    }
    public class UserListItemModel
    {
        public int UserId { get; set; }
        public string UserNameAndEmail { get; set; }
    }
    public class ChangePasswordModel
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordAgain { get; set; }
    }
    public class NewUserModel
    {
        public string UserEmail { get; set; }
        public string UserFullname { get; set; }
        public int UserRoles { get; set; }
        public string UserPassword { get; set; }
        public string PasswordSalt { get; set; }
    }
}
