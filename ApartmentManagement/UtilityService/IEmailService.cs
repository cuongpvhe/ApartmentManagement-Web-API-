using ApartmentManagement.Dto;

namespace ApartmentManagement.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
