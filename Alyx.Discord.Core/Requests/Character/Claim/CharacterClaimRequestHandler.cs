using System.Text;
using Alyx.Discord.Db;
using Alyx.Discord.Db.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetStone.Api.Client;

namespace Alyx.Discord.Core.Requests.Character.Claim;

internal class CharacterClaimRequestHandler(DatabaseContext context, NetStoneApiClient netStoneApiClient)
    : IRequestHandler<CharacterClaimRequest, CharacterClaimRequestResponse>
{
    public async Task<CharacterClaimRequestResponse> Handle(CharacterClaimRequest request,
        CancellationToken cancellationToken)
    {
        var claims = await context.CharacterClaims.Where(x =>
                x.CharacterId == request.LodestoneId ||
                x.DiscordId == request.DiscordId)
            .OrderByDescending(x => x.Confirmed)
            .ToListAsync(cancellationToken);

        if (claims.FirstOrDefault(x =>
                x.Confirmed &&
                x.DiscordId == request.DiscordId &&
                x.CharacterId != request.LodestoneId) is not null)
        {
            // user has already claimed a different character
            return new CharacterClaimRequestResponse(CharacterClaimRequestStatus.UserAlreadyHasMainCharacter);
        }

        if (claims.FirstOrDefault(x => x.CharacterId == request.LodestoneId && x.Confirmed) is { } confirmedClaim)
        {
            // check whether existing confirmed claim is user's or someone else's
            // !! never return another user's code !!
            return confirmedClaim.DiscordId == request.DiscordId
                ? new CharacterClaimRequestResponse(CharacterClaimRequestStatus.AlreadyClaimedByUser,
                    confirmedClaim.Code)
                : new CharacterClaimRequestResponse(CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser);
        }

        // no confirmed claims, check if user created claim before
        if (claims.FirstOrDefault(x => x.DiscordId == request.DiscordId) is { } existingClaim)
        {
            // if existing claim is for this character

            if (existingClaim.CharacterId != request.LodestoneId)
            {
                // user's existing claim is for a different character -> delete existing and create new
                await DeleteExistingClaim(request.DiscordId, cancellationToken);
                var newClaim =
                    await CreateAndSaveNewCharacter(request.LodestoneId, request.DiscordId, cancellationToken);
                return new CharacterClaimRequestResponse(CharacterClaimRequestStatus.NewClaimCreated, newClaim.Code);
            }

            // existing claim is for this character -> check if user has added code to profile
            if (!await ValidateCodeAsync(request.LodestoneId, existingClaim.Code, cancellationToken))
            {
                // code has not been added to profile
                return new CharacterClaimRequestResponse(CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter,
                    existingClaim.Code);
            }

            // code has been added to profile, confirm claim
            await ConfirmClaimAsync(existingClaim, cancellationToken);
            return new CharacterClaimRequestResponse(CharacterClaimRequestStatus.ClaimConfirmed, existingClaim.Code);
        }

        {
            // claim does not exist yet, create
            var newClaim = await CreateAndSaveNewCharacter(request.LodestoneId, request.DiscordId, cancellationToken);

            return new CharacterClaimRequestResponse(CharacterClaimRequestStatus.NewClaimCreated, newClaim.Code);
        }
    }

    private static string GenerateNewCode()
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

    private async Task<MainCharacter> CreateAndSaveNewCharacter(string lodestoneId, ulong discordId,
        CancellationToken cancellationToken)
    {
        var newClaim = new MainCharacter
        {
            CharacterId = lodestoneId,
            Code = GenerateNewCode(),
            DiscordId = discordId
        };

        await context.CharacterClaims.AddAsync(newClaim, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return newClaim;
    }


    /// <summary>
    ///     Check whether code has been added to Lodestone profile.
    /// </summary>
    /// <param name="lodestoneId">Lodestone ID of character.</param>
    /// <param name="code">Code that user was supposed to add to character's Lodestone profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task<bool> ValidateCodeAsync(string lodestoneId, string code, CancellationToken cancellationToken)
    {
        var character = await netStoneApiClient.Character.GetAsync(lodestoneId, 0, cancellationToken);
        return character.Bio.Contains(code);
    }

    private Task<int> ConfirmClaimAsync(MainCharacter claim, CancellationToken cancellationToken)
    {
        claim.Confirmed = true;
        return context.SaveChangesAsync(cancellationToken);
    }

    private async Task DeleteExistingClaim(ulong discordId, CancellationToken cancellationToken)
    {
        var claim = await context.CharacterClaims.FirstOrDefaultAsync(x => x.DiscordId == discordId, cancellationToken);
        if (claim is null)
        {
            throw new InvalidOperationException("No claim exists for given Discord user ID");
        }

        if (claim.Confirmed)
        {
            throw new InvalidOperationException("Claim was confirmed before. It must be manually deleted.");
        }

        context.CharacterClaims.Remove(claim);
        await context.SaveChangesAsync(cancellationToken);
    }
}