internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();


        var characterSet = "ACDEFGHKLMNPRTXYZ234579";
        var codeLength = 8;
        var random = new Random();

        var uniqueCode = new string(Enumerable.Repeat(characterSet, codeLength)
          .Select(s => s[random.Next(s.Length)]).ToArray());

        Console.WriteLine(uniqueCode);


        app.Run();
    }
}