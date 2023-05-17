using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;

namespace spending_money_control_api
{
    class Program
    {
        static void Main(string[] args)
        {
            constants.mongoClient = new MongoClient("mongodb+srv://user1:qwerty123@cluster0.vkmbyoc.mongodb.net/test");
            constants.database = constants.mongoClient.GetDatabase("base1");
            constants.collection = constants.database.GetCollection<BsonDocument>("tg-names");
            constants.collection2 = constants.database.GetCollection<BsonDocument>("collection1");




            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // добавление Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                // задание параметров Swagger
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
            });

            var app = builder.Build();

            // настройка Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }
    }
}