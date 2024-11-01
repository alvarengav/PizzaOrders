using Microsoft.AspNetCore.DataProtection;

namespace PizzaOrders.GraphQLApi.Services;

public class DataProtectionEncryptionService : IEncryptionService
{
    private readonly IDataProtector _protector;

    public DataProtectionEncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("CustomerDataProtector");
    }

    public string Encrypt(string input)
    {
        return _protector.Protect(input);
    }

    public string Decrypt(string encryptedInput)
    {
        return _protector.Unprotect(encryptedInput);
    }
}
