namespace PizzaOrders.GraphQLApi.Services;

public interface IEncryptionService
{
    string Encrypt(string input);
    string Decrypt(string encryptedInput);
}
