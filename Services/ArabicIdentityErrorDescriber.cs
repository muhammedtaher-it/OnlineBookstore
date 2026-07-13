using Microsoft.AspNetCore.Identity;

namespace OnlineBookstore.Services
{
    public class ArabicIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "يجب أن تحتوي كلمة المرور على حرف خاص واحد على الأقل (مثل !@#$%)."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل (0-9)."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل (a-z)."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل (A-Z)."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"كلمة المرور قصيرة جداً. يجب أن تكون {length} أحرف على الأقل."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"اسم المستخدم أو البريد الإلكتروني '{userName}' مسجل مسبقاً."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"البريد الإلكتروني '{email}' مسجل مسبقاً."
            };
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = "حدث خطأ غير معروف."
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"اسم المستخدم '{userName}' غير صالح، يمكن أن يحتوي فقط على أحرف وأرقام."
            };
        }
    }
}