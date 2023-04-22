namespace Cave;

/// <summary>Available root locations.</summary>
public enum RootLocation
{
    /// <summary>The roaming user data folder</summary>
    RoamingUserData = 0,

    /// <summary>The roaming user configuration folder</summary>
    RoamingUserConfig,

    /// <summary>The local user data folder</summary>
    LocalUserData,

    /// <summary>The local user configuration folder</summary>
    LocalUserConfig,

    /// <summary>All users data folder</summary>
    AllUsersData,

    /// <summary>All user configuration folder</summary>
    AllUserConfig,

    /// <summary>The program installation folder</summary>
    Program
}
