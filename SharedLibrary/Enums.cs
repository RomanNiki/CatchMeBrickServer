namespace SharedLibrary
{
    public enum ServerToClientId : ushort
    {
        Sync = 1,
        ActiveScene,
        PlayerSpawned,
        PlayerMovement,
        PlayerHealthChanged,
        PlayerActiveWeaponUpdated,
        PlayerAmmoChanged,
        PlayerDied,
        PlayerRespawned,
        ProjectileSpawned,
        ProjectileMovement,
        ProjectileCollided,
        ProjectileHitMarker,
    }

    public enum ClientToServerId : ushort
    {
        Name = 1,
        Input,
        SwitchActiveWeapon,
        PrimaryUse,
        Reload,
    }
    
    public enum WeaponType
    {
        Brick = 0,
    }
    
    public enum Team
    {
        None = 0,
        Red,
        Green
    }
}