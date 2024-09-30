using System.Text.Json;
using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.InteractionData.Get;

public class InteractionDataGetRequestHandler<T>(DatabaseContext context)
    : IRequestHandler<InteractionDataGetRequest<T>, T>
{
    public async Task<T> Handle(InteractionDataGetRequest<T> request, CancellationToken cancellationToken)
    {
        var data = await context.InteractionDatas.FirstAsync(x => x.Key == request.Key,
            cancellationToken);

        if (data.Type != request.Type.FullName)
        {
            throw new ArgumentException("Interaction data type does not match");
        }

        var des = JsonSerializer.Deserialize<T>(data.Value);

        if (des is null)
        {
            throw new InvalidOperationException("Interaction data is empty");
        }

        return des;
    }
}