using MediatR;

namespace Alyx.Discord.Core.Requests.Ffxiv.Copypasta;

public record FfxivCopypastaRequest : IRequest<FfxivCopypastaRequestResponse>;