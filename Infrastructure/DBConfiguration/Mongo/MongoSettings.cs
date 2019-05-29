using Domain.Entities;
using Infrastructure.Interfaces.DBConfiguration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infrastructure.DBConfiguration.Mongo
{
    public class MongoSettings : IMongoSettings
    {
        public string DefaultConnection { get; set; }
        public string DBName { get; set; }

        public static void ConfigureMongoConventions()
        {
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;

            var serializer = new DateTimeSerializer(DateTimeKind.Local);
            BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);

            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.MapCreator(x => User.UserFactory.NewUserFactory(x.Id, x.TasksToDo.ToArray()));
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Name).SetIsRequired(true);
                map.MapMember(x => x.TasksToDo).SetElementName(nameof(TaskToDo)).SetDefaultValue(new Collection<TaskToDo>());
                //map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<TaskToDo>(map =>
            {
                map.AutoMap();
                map.UnmapMember(x => x.User);
                map.UnmapMember(x => x.UserId);
            });
        }
    }
}
