using MediatR;

namespace Alyx.Discord.Core.Requests.Character.GetMainCharacterId;

public record GetMainCharacterIdRequest(ulong DiscordId) : IRequest<string>;