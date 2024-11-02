using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PizzaOrders.GraphQLApi.Services;

namespace PizzaOrders.GraphQLApi.Utils;

public class EncryptedConverter : ValueConverter<string, string>
{
    public EncryptedConverter(IEncryptionService encryptionService)
        : base(v => encryptionService.Encrypt(v), v => encryptionService.Decrypt(v)) { }
}
