
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres");
var rabbitMq = builder.AddRabbitMQ("eventbus");
var mongodb = builder.AddMongoDB("mongo");

var courseDb = mongodb.AddDatabase("coursedb");
var instructorDb = postgres.AddDatabase("instructordb");
var studentDb = postgres.AddDatabase("studentdb");

builder.AddProject<Projects.Course_Api>("course-api")
    .WithReference(redis)
    .WithReference(courseDb)
    .WithReference(rabbitMq);

builder.AddProject<Projects.Instructor_Api>("instructor-api")
    .WithReference(instructorDb)
    .WithReference(rabbitMq);

builder.AddProject<Projects.Student_Api>("student-api")
    .WithReference(redis)
    .WithReference(studentDb)
    .WithReference(rabbitMq);

builder.Build().Run();
