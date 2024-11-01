# Pizza Orders API

## Requirements

- Docker
- .NET 8
- Visual Studio 2022 (project setup and testing are currently only verified in Visual Studio 2022)

## Project Setup

1. **Environment Configuration**\
   Create a `.env` file in the project root with the following variables:

   ```
   MYSQL_ROOT_PASSWORD=password
   MYSQL_DATABASE=database
   MYSQL_USER=user
   MYSQL_PASSWORD=password
   ```

2. **Connection String Configuration**\
   Update `appsettings.json` to include the connection string, replacing the placeholders with values from the `.env` file:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=dbserver;Database=PizzaOrderDb;User=user;Password=password;"
   }
   ```

3. **Database Migration (Optional)**\
   To configure the environment variable for running migrations, use the following command:

   ```
   $Env:ConnectionStrings__DefaultConnection="Server=127.0.0.1;Port=3306;Database=PizzaOrderDb;User=root;Password=password";
   ```

   Replace the placeholders with the values from your `.env` file. This step is optional because the migration is already included in the code, and would only be necessary if more migrations are added.

   The following commands to set up the database are also optional and only required if adding new migrations:

   ```
   dotnet tool install --global dotnet-ef
   dotnet ef database update
   ```

4. **Run Locally**\
   This project has only been verified to run using Visual Studio 2022. To run the project, open it in Visual Studio and use Docker Compose from the Visual Studio interface:

   ![Docker Compose](./docs/docker-compose-visual-studio.png)

   **Note:** Do not use `docker-compose up -d` from the command line to run the project, as it has only been configured and tested to run within Visual Studio.

   **Note:** When running locally, you might encounter a "Not Found" page initially. Navigate to `/graphql` to access the GraphQL playground.

## Running Tests

To execute tests, you can either run:

```
   dotnet test
```

or use the **Test Explorer** in Visual Studio 2022 to run and manage the tests.

## Deployment Instructions

The project is currently deployed on Azure using Docker Registry and Web App. To replicate the deployment, you would need to:

1. Set up a Docker Container Registry.
2. Set up an Azure Web App to use the image from the Docker Registry.
3. Set up a MySQL database on Azure.
4. Configure the necessary environment variables:
   - `MYSQL_ROOT_PASSWORD`
   - `MYSQL_DATABASE`
   - `MYSQL_USER`
   - `MYSQL_PASSWORD`
     Unfortunately, CI/CD was not implemented due to time constraints.

The project can be accessed at: [Pizza Orders GraphQL API](https://pizzaorders-esapgwa3f9a2a5gr.eastus2-01.azurewebsites.net/graphql/)

## Design Considerations

- **Database**: Uses MySQL for persistent data storage.
- **GraphQL API**: Built with Hot Chocolate for flexible data access.
- **Encryption**: Customer data is encrypted for security.
- **Design Decisions**: The models are not fully normalized to prioritize simplicity in creating orders in the GraphQL playground. Instead of handling multiple IDs (e.g., `pizzaId`, `toppingId`, `customerId`), the database design allows direct input of customer and pizza details in a more user-friendly way. For instance, the following mutation allows you to create an order without having to create multiple records beforehand:

  ```graphql
  mutation {
    createOrder(
      input: {
        customer: {
          name: "Carlos Perez"
          phone: "456-321-8080"
          address: "123 Main St, Ciudad"
        }
        pizza: {
          type: PEPPERONI
          size: MEDIUM
          toppings: [PINEAPPLE, SAUSAGE, SPINACH]
        }
      }
    ) {
      order {
        id
      }
    }
  }
  ```
