using Ecard;

namespace PI8583
{
    public interface IDealServiceFactory
    {
        IAccountDealService CreateService(DatabaseInstance databaseInstance);
    }
}
