using Domain.Entities.Users;
using Infrastructure.MongoDb.Mappings;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Repositories;

public class UsersRepository(IMongoDatabase database) : BaseEntityRepository<User, UserMapping>(database);