using MediatR;

namespace Alyx.Discord.Core.Requests.General.Statistics;

public record StatisticsRequest : IRequest<StatisticsRequestResponse>;