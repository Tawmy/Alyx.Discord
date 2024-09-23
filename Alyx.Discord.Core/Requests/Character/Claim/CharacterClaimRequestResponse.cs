namespace Alyx.Discord.Core.Requests.Character.Claim;

public readonly record struct CharacterClaimRequestResponse(CharacterClaimRequestStatus Status, string? Code = null);

public enum CharacterClaimRequestStatus
{
    /// <summary>
    ///     The user has already claimed a different character. A user can only have one main character.
    /// </summary>
    UserAlreadyHasMainCharacter,

    /// <summary>
    ///     Character has already been claimed by user.
    /// </summary>
    AlreadyClaimedByUser,

    /// <summary>
    ///     Character has already been claimed by a different user.
    /// </summary>
    AlreadyClaimedByDifferentUser,

    /// <summary>
    ///     Claim for this character already exists for user, but it has not been confirmed with code yet.
    /// </summary>
    ClaimAlreadyExistsForThisCharacter,

    /// <summary>
    ///     Claim for another character already exists for this user, but it has not been confirmed with code yet.
    /// </summary>
    ClaimAlreadyExistsForDifferentCharacter,

    /// <summary>
    ///     Claim for this character already exists, and code was found on Lodestone profile.
    /// </summary>
    ClaimConfirmed,

    /// <summary>
    ///     Claim for this character did not exist for user, and a new one has been created.
    /// </summary>
    NewClaimCreated
}