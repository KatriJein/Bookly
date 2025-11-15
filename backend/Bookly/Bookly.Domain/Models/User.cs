using Core;
using Core.Dto.User;
using Core.Enums;
using Core.ValueObjects;

namespace Bookly.Domain.Models;

public class User : Entity<Guid>
{
    public Login Login { get; private set; }
    public Email Email { get; private set; }
    public string? AvatarKey { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }
    public AgeCategory? AgeCategory  { get; private set; }
    public VolumeSizePreference? VolumeSizePreference  { get; private set; }
    public bool HatedGenresStrictRestriction { get; private set; }
    public string PasswordHash { get; private set; }
    public bool TookEntrySurvey { get; private set; }
    
    private readonly List<UserGenrePreference> _userGenrePreferences = [];
    private readonly List<UserAuthorPreference> _userAuthorPreferences = [];
    
    public IReadOnlyCollection<UserGenrePreference> UserGenrePreferences => _userGenrePreferences;
    public IReadOnlyCollection<UserAuthorPreference> UserAuthorPreferences => _userAuthorPreferences;

    public static Result<User> Create(CreateUserDto createUserDto)
    {
        var user = new User();
        var loginRes = user.SetLogin(createUserDto.Login);
        if (loginRes.IsFailure) return Result<User>.Failure(loginRes.Error);
        var emailRes = user.SetEmail(createUserDto.Email);
        if (emailRes.IsFailure) return Result<User>.Failure(emailRes.Error);
        user.SetPasswordHash(createUserDto.PasswordHash);
        user.CreatedAt = DateTime.UtcNow;
        return Result<User>.Success(user);
    }

    public Result SetLogin(string login)
    {
        var newLogin = Login.Create(login);
        if (newLogin.IsFailure) return Result.Failure(newLogin.Error);
        Login = newLogin.Value;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result SetEmail(string email)
    {
        var newEmail = Email.Create(email);
        if (newEmail.IsFailure) return Result.Failure(newEmail.Error);
        Email = newEmail.Value;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void SetAvatarKey(string? avatarKey)
    {
        AvatarKey = avatarKey;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddGenrePreferences(IEnumerable<UserGenrePreference> userGenrePreferences) =>
        _userGenrePreferences.AddRange(userGenrePreferences);

    public void AddAuthorPreferences(IEnumerable<UserAuthorPreference> userAuthorPreferences) =>
        _userAuthorPreferences.AddRange(userAuthorPreferences);

    public void MarkEntrySurveyTaken() => TookEntrySurvey = true;

    public void SetAgeCategory(AgeCategory ageCategory) => AgeCategory = ageCategory;
    public void SetHatedGenresRestriction(bool isBlacklist) => HatedGenresStrictRestriction = isBlacklist;

    public void SetVolumeSizePreference(VolumeSizePreference volumeSizePreference) =>
        VolumeSizePreference = volumeSizePreference;
}