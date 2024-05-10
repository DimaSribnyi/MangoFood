# MangoFood

https://mangofoodweb.azurewebsites.net

The site is written in Microservice architecture style. For the front-end, I used the MVC pattern with Razor Pages. Account management and security are made with JWT and Microsoft Identity. Ocelot Gateway authorizes and routes microservices. All services are written in a RESTful style. Each service has its own database. CRUD operations, with only admins' access to modify entities. EF Core as an ORM, with Azure SQL in production and MS SQL in development. The Azure Service Bus is used for asynchronous communication. Stripe test mode is used as a payment service. The site was deployed to Azure Cloud Service, where it is hosted.
