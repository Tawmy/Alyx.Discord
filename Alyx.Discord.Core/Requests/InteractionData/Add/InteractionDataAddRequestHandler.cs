using System.Text.Json;
using Alyx.Discord.Db;
using MediatR;

namespace Alyx.Discord.Core.Requests.InteractionData.Add;

public class InteractionDataAddRequestHandler<T>(DatabaseContext context)
    : IRequestHandler<InteractionDataAddRequest<T>, InteractionDataAddResponse>
{
    public async Task<InteractionDataAddResponse> Handle(InteractionDataAddRequest<T> request,
        CancellationToken cancellationToken)
    {
        var guid = Guid.CreateVersion7();

        await context.InteractionDatas.AddAsync(new Db.Models.InteractionData
        {
            Key = guid.ToString(),
            Value = JsonSerializer.Serialize(request.Value),
            Type = request.Type.FullName ??
                   throw new ArgumentNullException(nameof(request), "type full name cannot be null.")
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return new InteractionDataAddResponse(guid);
    }
}