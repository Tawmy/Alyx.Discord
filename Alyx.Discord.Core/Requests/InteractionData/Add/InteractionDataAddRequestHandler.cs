using System.Text;
using System.Text.Json;
using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.InteractionData.Add;

public class InteractionDataAddRequestHandler<T>(DatabaseContext context)
    : IRequestHandler<InteractionDataAddRequest<T>, InteractionDataAddResponse>
{
    public async Task<InteractionDataAddResponse> Handle(InteractionDataAddRequest<T> request,
        CancellationToken cancellationToken)
    {
        string key;
        do
        {
            // ensure no duplicate key is generated
            key = GenerateNewId();
        } while (await context.InteractionDatas.AnyAsync(x => x.Key == key, cancellationToken));

        await context.InteractionDatas.AddAsync(new Db.Models.InteractionData
        {
            Key = key,
            Value = JsonSerializer.Serialize(request.Value),
            Type = request.Type.FullName ??
                   throw new ArgumentNullException(nameof(request), "type full name cannot be null.")
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return new InteractionDataAddResponse(key);
    }

    private static string GenerateNewId()
    {
        var stringBuilder = new StringBuilder();

        // generate a 32 characters long random string
        for (var i = 0; i < 4; i++)
        {
            var path = Path.GetRandomFileName();
            stringBuilder.Append(path[..8]);
        }

        return stringBuilder.ToString();
    }
}